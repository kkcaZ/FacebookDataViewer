using FacebookDataViewer.Data.Dto;

namespace FacebookDataViewer.Services.Interfaces;

/// <summary>
/// Communicates with MongoDB
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Upload image to database
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="base64"></param>
    void UploadImage(string uri, string base64);

    /// <summary>
    /// Upload message json to database
    /// </summary>
    /// <param name="title"></param>
    /// <param name="conversation"></param>
    void UploadMessages(string title, ConversationDto conversation); 
    
    /// <summary>
    /// Tries to get image from database using uri as identifier
    /// </summary>
    /// <param name="uri">identifier for image file</param>
    /// <param name="base64">image file returned from db</param>
    /// <returns>true if image is found</returns>
    bool TryGetImage(string uri, out string base64);

    /// <summary>
    /// Tries to get conversation from database using title as identifier
    /// </summary>
    /// <param name="title">identifier for message json</param>
    /// <param name="conversation">messages returned from db</param>
    /// <returns>true if messages are found</returns>
    bool TryGetMessages(string title, out ConversationDto conversation);

    /// <summary>
    /// Gets all conversations from db
    /// </summary>
    /// <returns></returns>
    List<string> GetAllConversations();
}