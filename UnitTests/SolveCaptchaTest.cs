using System.Activities;
using Smarthis.UiPathLib.Activities;
using Smarthis.UiPathLib.Activities.Extension;
using Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Enum;


namespace UnitTests
{
    public static class SolveCaptchaTest
    {
        public static void UnitTest()
        {
            var textCaptchaArguments = new Dictionary<string, object>
        {
            { "Username", "" },
            { "Password", "".ToSecureString() },
            {"ImageUrl","https://nopecha.com/image/demo/textcaptcha/1aCtB.png" },
        };
           RunTest(textCaptchaArguments, CaptchaTypes.TextCaptcha);

            var reCaptchaArguments = new Dictionary<string, object>
        {
            { "Username", "" },
            { "Password", "".ToSecureString() },
            { "SiteUrl", "https://www.sefaz.rs.gov.br/sat/CertidaoSitFiscalSolic.aspx" },
            { "CaptchaKey", "6LeQHh8TAAAAAIy0vAtOHLm62yhbmWxT9_HUoAnh" },
        };
            RunTest(reCaptchaArguments, CaptchaTypes.ReCaptcha);


            var hCaptchaArguments = new Dictionary<string, object>
        {
            { "Username", "" },
            { "Password", "".ToSecureString() },
            { "SiteUrl", "https://sicalc.receita.economia.gov.br/sicalc/rapido/contribuinte" },
            { "CaptchaKey", "20a82aa0-d5ea-4ae2-8b4c-ac341dfe6ee7" },
        };
            RunTest(hCaptchaArguments, CaptchaTypes.HCaptcha);
          
            
        }

        private static void RunTest(Dictionary<string, object> arguments, CaptchaTypes captcha)
        {
            var activity = new SolveCaptcha()
            {
                CaptchaType = captcha,
            };
            var response = WorkflowInvoker.Invoke(activity, arguments, new TimeSpan(0, 0, 2, 30));
        }
    }

}
