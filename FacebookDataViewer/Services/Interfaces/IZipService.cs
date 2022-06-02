namespace FacebookDataViewer.Services.Interfaces;

/// <summary>
/// Carries out Archive functions
/// </summary>
public interface IZipService
{
    /// <summary>
    /// Gets all information from Facebook archive & uploads to DB
    /// </summary>
    /// <param name="stream">stream from zip file</param>
    /// <returns>List of all conversations in archive</returns>
    Task<List<string>> UnzipArchive(Stream stream);
}