using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Smarthis.UiPathLib.Activities.Extension;

public static class SecureStringExtensions
{
    public static string ToPlainString(this SecureString value)
    {
        var valuePtr = IntPtr.Zero;
        try
        {
            valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
            return Marshal.PtrToStringUni(valuePtr);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
        }
    }
}