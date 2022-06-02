namespace FacebookDataViewer.Helpers;

public static class StringHelper
{
    /// <summary>
    /// Checks if given file path is an image
    /// </summary>
    /// <param name="str"></param>
    /// <returns>true if file is an image</returns>
    public static bool IsImageFile(this string str)
    {
        if (str.Contains(".jpg")
            || str.Contains(".jpeg")
            || str.Contains(".png")
            || str.Contains(".gif"))
            return true;
        else
            return false;
    }
}