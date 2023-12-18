using System.Activities;
using System.Security;
using Smarthis.UiPathLib.Activities;

namespace UnitTests;

public static class SecureStringToStringTest
{
    public static void UnitTest()
    {    
        var secureString = new SecureString();
        "dumbPassword".ToCharArray().ToList().ForEach(letter => secureString.AppendChar(letter));
        var arguments = new Dictionary<string, object>
        {
            { "SecuredString", secureString }
        };
        var activity = new SecureStringToString()
        {
            SecuredString = secureString

        };
            
        var response = WorkflowInvoker.Invoke(activity, arguments, new TimeSpan(0,0,0, 5));

        var unsecuredString = (string) response["UnsecuredString"];
        if (!unsecuredString.Equals("dumbPassword"))
            throw new ApplicationException();
    }
}