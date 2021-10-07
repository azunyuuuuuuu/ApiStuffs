public static class ExtensionMethods
{
    // from https://stackoverflow.com/a/40917033
    static public string ToShortString(this Guid guid)
    {
        var base64Guid = Convert.ToBase64String(guid.ToByteArray());

        // Replace URL unfriendly characters with better ones
        base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

        // Remove the trailing ==
        return base64Guid.Substring(0, base64Guid.Length - 2);
    }

    static public Guid ToGuidFromShortString(this string str)
    {
        str = str.Replace('_', '/').Replace('-', '+');
        var byteArray = Convert.FromBase64String(str + "==");
        return new Guid(byteArray);
    }
}
