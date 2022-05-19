namespace R4Cheat;

using System.Text;

public static class Misc
{
    public static Encoding TryGetEncoding(string encodingName)
    {
        try
        {
            return Encoding.GetEncoding(encodingName);
        }
        catch
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(encodingName);
        }
    }
}