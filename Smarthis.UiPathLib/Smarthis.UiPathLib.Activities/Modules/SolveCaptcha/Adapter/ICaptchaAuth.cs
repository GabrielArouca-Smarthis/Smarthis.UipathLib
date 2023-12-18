using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Endpoints;


namespace Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Adapter
{
    public interface ICaptchaAuth
    {
        [Post(path: CaptchaEndpoints.EndpointAuth )]
        Task<ApiResponse<string>> GetCaptchaAccessToken([Body] FormUrlEncodedContent data);
    }
}
