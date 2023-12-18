using Refit;
using Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Endpoints;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Adapter;



public interface ICaptchaSolver
{
    [Post(path: CaptchaEndpoints.EndpointHcaptcha)]
    Task<ApiResponse<string>> GetHcaptchaResolutionId([Body] Dictionary<string, string> data, [Authorize] string acessToken);

    [Post(path: CaptchaEndpoints.EndpointRecaptcha)]
    Task<ApiResponse<string>> GetRecaptchaResolutionId([Body] Dictionary<string, string> data, [Authorize] string acessToken);

    [Multipart]
    [Post(path: CaptchaEndpoints.EndpointTextcaptcha)]
    Task<ApiResponse<string>> GetTextcaptchaResolutionId( ByteArrayPart captcha, [Authorize] string acessToken);

    [Get(path: CaptchaEndpoints.EndpointResolutionToken)]
    Task<ApiResponse<string>> GetCaptchaResolutionToken( string resolutionId, [Authorize] string acessToken);


}
