namespace FacebookDataViewer.Extensions;

public static class StreamExtension
{
    /// <summary>
    /// Reads all bytes of stream
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static async Task<byte[]> ReadFully(this Stream input)
    {
        await using var ms = new MemoryStream();
        await input.CopyToAsync(ms);
        return ms.ToArray();
    }
}