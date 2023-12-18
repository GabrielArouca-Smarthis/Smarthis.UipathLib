using System.Linq;
using System.Security;

namespace Smarthis.UiPathLib.Activities.Extension;

public static class StringExtensions
{
    public static SecureString ToSecureString(this string value)
    {
        var securedString = new SecureString();
        value.ToCharArray().ToList().ForEach(c => securedString.AppendChar(c));
        return securedString;
    }
}