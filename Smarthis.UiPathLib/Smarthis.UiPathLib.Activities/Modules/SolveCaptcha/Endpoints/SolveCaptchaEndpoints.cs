namespace Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Endpoints
{
    public class CaptchaEndpoints
    {
        public const string EndpointHcaptcha = "/api/v1/hcaptcha";
        public const string EndpointRecaptcha = "/api/v1/recaptcha";
        public const string EndpointTextcaptcha = "/api/v1/captcha";
        public const string EndpointResolutionToken = "/api/v1/captcha/{resolutionId}";
        public const string EndpointAuth = "/realms/ProdGeneral/protocol/openid-connect/token";
    }
}
