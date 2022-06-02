using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace FacebookDataViewer.Data.Dto;

/// <summary>
/// Facebook Conversation Data Structure
/// </summary>
public class ConversationDto
{
    public string Title { get; set; } = string.Empty;

    public List<User> Participants { get; set; } = new List<User>();

    public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
}

/// <summary>
/// Facebook User Data Structure
/// </summary>
public class User
{
    public string Name { get; set; } = string.Empty;
}