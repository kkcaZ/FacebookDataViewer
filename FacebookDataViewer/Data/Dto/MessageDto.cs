using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace FacebookDataViewer.Data.Dto;

/// <summary>
/// Facebook Singular Message Data Structure
/// </summary>
[BsonDiscriminator("Message")]
public class MessageDto
{
    public string Sender_Name { get; set; } = "Unknown";

    public long Timestamp_Ms { get; set; } = 0;

    public string? Content { get; set; } = null;

    public List<Photo>? Photos { get; set; } = null;

    public Sticker? Sticker { get; set; } = null;

    public string Type { get; set; } = string.Empty;

    public bool Is_Unsent { get; set; } = false;

    public bool Is_Taken_Down { get; set; } = false;
}

public class Photo
{
    public string Uri { get; set; } = string.Empty;

    public long Creation_Timestamp { get; set; } = 0;
}

public class Sticker
{
    public string Uri { get; set; } = String.Empty;
}