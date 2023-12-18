using System.Activities;
using System.Security;
using Smarthis.UiPathLib.Activities;
using Smarthis.UiPathLib.Activities.Extension;

namespace UnitTests;

public static class StringToSecureStringTest
{
    public static void UnitTest()
    {
        var arguments = new Dictionary<string, object>
        {
            { "UnsecuredString", "dumbPassword" }
        };
        var activity = new StringToSecureString();
        
        var response = WorkflowInvoker.Invoke(activity, arguments, new TimeSpan(0,0,0, 5));

        var securedString = (SecureString) response["SecuredString"];
        if (!securedString.ToPlainString().Equals("dumbPassword"))
            throw new ApplicationException();
    }
}