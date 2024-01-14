namespace ShopManager.Client.Common;

public static class GuidHelper
{
    public static string ToBase64String(this Guid guid)
    {
        return Convert.ToBase64String(guid.ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .Substring(0, 22);
    }
    
    public static Guid FromBase64String(string base64String)
    {
        base64String = base64String.Replace("_", "/")
            .Replace("-", "+");
        var byteArray = Convert.FromBase64String(base64String + "==");
        return new Guid(byteArray);
    }
}