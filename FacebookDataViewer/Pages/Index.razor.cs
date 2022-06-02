using System.IO.Compression;
using System.Text;
using System.Text.Json;
using FacebookDataViewer.Data;
using FacebookDataViewer.Data.Dto;
using FacebookDataViewer.Extensions;
using FacebookDataViewer.Helpers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using FacebookDataViewer.Services;
using FacebookDataViewer.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FacebookDataViewer.Pages;

public partial class Index
{
    // Services
    [Inject] IScrollInfoService _scrollInfoService { get; set; }
    [Inject] IDatabaseService _databaseService { get; set; }
    [Inject] IZipService _zipService { get; set; }
    [Inject] ILogger _logger { get; set; }
    
    // Messages
    private List<string> Messages { get; set; }
    private ConversationDto SelectedConversation { get; set; }
    private ConversationDto FilteredConversation { get; set; }
    
    private int LoadedMessageCount = 30;
    
    // Filters
    private Dictionary<string, bool> _participantFilter = null;
    private bool _textFilter = true;
    private bool _imageFilter = true;
    private bool _stickerFilter = true;

    // Initialisation
    protected override Task OnInitializedAsync()
    {
        _scrollInfoService.OnScroll += OnMessageScroll;
        return base.OnInitializedAsync();
    }

    /// <summary>
    /// When file uploaded, downloads data & uploads it to db
    /// </summary>
    /// <param name="e"></param>
    private async Task FileChanged(InputFileChangeEventArgs e)
    {
        using (var stream = e.File.OpenReadStream(1000000000))
            Messages = await _zipService.UnzipArchive(stream);

        Messages.Sort();
        ChangeConversation(Messages[0]);
        StateHasChanged();
    }

    /// <summary>
    /// Lazy load messages
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnMessageScroll(object sender, ScrollEventArgs e)
    {
        if (e.ScrollTop > e.ScrollHeight - 1000)
        {
            LoadedMessageCount += 30;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Encodes emojis correctly
    /// </summary>
    /// <param name="text">Emoji text to be fixed</param>
    /// <returns>Correctly encoded emoji</returns>
    public string FixEmoji(string text)
    {
        return Encoding.UTF8.GetString(Encoding.Latin1.GetBytes(text));
    }

    /// <summary>
    /// Change conversation that is currently being viewed
    /// </summary>
    /// <param name="msg"></param>
    private void ChangeConversation(string msg)
    {
        _participantFilter = new Dictionary<string, bool>();
        
        if (_databaseService.TryGetMessages(msg, out ConversationDto msgs))
            SelectedConversation = msgs;
        else
            _logger.LogWarning("Could not find selected conversation in database");

        // Reset participant filters
        foreach (var participant in SelectedConversation.Participants)
            _participantFilter.Add(participant.Name, true);
        
        FilterMessages(SelectedConversation.Messages);
        StateHasChanged();
    }

    /// <summary>
    /// Retrieves Conversations from DB and stores in global Messages variable
    /// </summary>
    private void GetConversationsFromDb()
    {
        Messages = _databaseService.GetAllConversations();
        Messages.Sort();
        ChangeConversation(Messages[0]);
    }

    /// <summary>
    /// Updates participant filter when checkbox clicked
    /// </summary>
    /// <param name="e"></param>
    /// <param name="participant"></param>
    private void ParticipantCheckboxChanged(ChangeEventArgs e, string participant)
    {
        _participantFilter[participant] = !_participantFilter[participant];
        FilterMessages(SelectedConversation.Messages);
    }

    /// <summary>
    /// Updates media filter when checkbox clicked
    /// </summary>
    /// <param name="e"></param>
    /// <param name="media"></param>
    private void MediaCheckboxChanged(ChangeEventArgs e, string media)
    {
        switch (media)
        {
            case "Text":
                _textFilter = !_textFilter;
                break;
            case "Images":
                _imageFilter = !_imageFilter;
                break;
            case "Stickers":
                _stickerFilter = !_stickerFilter;
                break;
        }
        FilterMessages(SelectedConversation.Messages);
    }

    /// <summary>
    /// Filters messages depending on selected filters - outputs to FilteredMessages variable
    /// </summary>
    /// <param name="msgs"></param>
    private void FilterMessages(List<MessageDto> msgs)
    {
        // Clone list to remove any cross referencing
        var newMsgs = msgs.ToList();
        
        // Removes hidden participants
        foreach (KeyValuePair<string, bool> kvp in _participantFilter)
            if (kvp.Value == false)
                newMsgs.RemoveAll(m => m.Sender_Name == kvp.Key);

        // Removes all images if filter off
        if (!_imageFilter)
            newMsgs.RemoveAll(m => m.Photos != null);
        
        // Removes all stickers if filter off
        if (!_stickerFilter)
            newMsgs.RemoveAll(m => m.Sticker != null);
        
        // Removes all texts if filter off
        if (!_textFilter)
            newMsgs.RemoveAll(m => m.Content != null);
        
        FilteredConversation = new ConversationDto()
        {
            Participants = SelectedConversation.Participants,
            Messages = newMsgs
        };
    }
}