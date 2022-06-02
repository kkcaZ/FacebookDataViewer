namespace FacebookDataViewer.Data;

/// <summary>
/// Archive Zip Entry Data Structure
/// </summary>
public record ZipEntry()
{
    public string Name { get; init; }
    
    public string Content { get; init; }
}