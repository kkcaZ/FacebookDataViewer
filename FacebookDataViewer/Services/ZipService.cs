using System.IO.Compression;
using System.Text;
using System.Text.Json;
using FacebookDataViewer.Data;
using FacebookDataViewer.Data.Dto;
using FacebookDataViewer.Extensions;
using FacebookDataViewer.Helpers;
using FacebookDataViewer.Services.Interfaces;
using Microsoft.AspNetCore.Components;


namespace FacebookDataViewer.Services;

/// <inheritdoc />
public class ZipService : IZipService
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<ZipService> _logger;
    
    public ZipService(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }
    
    /// <inheritdoc />
    public async Task<List<string>> UnzipArchive(Stream stream)
    {
        await using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        using var archive = new ZipArchive(ms);

        var entries = new List<ZipEntry>();

        foreach (var entry in archive.Entries)
        {
            await using var fileStream = entry.Open();
            var fileBytes = await fileStream.ReadFully();

            var content = "";
            if (entry.FullName.IsImageFile())
            {
                if (!_databaseService.TryGetImage(entry.FullName, out _))
                {
                    content = Convert.ToBase64String(fileBytes);
                    _databaseService.UploadImage(entry.FullName, content);
                }
            }
            else if (entry.FullName.Contains(".json"))
            {
                content = Encoding.UTF8.GetString(fileBytes);
                entries.Add(new ZipEntry() { Name=entry.FullName, Content = content });
                _logger.LogInformation("Name: {0}", entry.FullName);
            }
        }
        
        var JsonFiles = entries.Where(x => x.Name.Contains(".json")).ToList();
        var chats = new Dictionary<string, ConversationDto>();
        
        // Collate messages
        foreach (var json in JsonFiles)
        {
            var msg = JsonSerializer.Deserialize<ConversationDto>(json.Content, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true 
            });

            if (chats.ContainsKey(msg.Title))
                chats[msg.Title].Messages.AddRange(msg.Messages);
            else
                chats.Add(msg.Title, msg);
        }

        List<string> Messages = new List<string>();
        
        foreach (KeyValuePair<string, ConversationDto> kvp in chats) {
            Messages.Add(kvp.Key);
            if (!_databaseService.TryGetMessages(kvp.Value.Title, out ConversationDto msgs))
            {
                _databaseService.UploadMessages(kvp.Value.Title, kvp.Value);
            }
        }

        // Dispose of resources
        await ms.DisposeAsync();
        await stream.DisposeAsync();
        archive.Dispose();

        return Messages;
    }
}