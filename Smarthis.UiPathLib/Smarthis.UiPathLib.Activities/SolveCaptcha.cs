using Refit;
using Smarthis.UiPathLib.Activities.Extension;
using Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Adapter;
using Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Enum;
using Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Exception;
using Smarthis.UiPathLib.Activities.Properties;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;



namespace Smarthis.UiPathLib.Activities;

[LocalizedDisplayName(nameof(Resources.SolveCaptcha_DisplayName))]
[LocalizedDescription(nameof(Resources.SolveCaptcha_Description))]

public class SolveCaptcha : ContinuableAsyncCodeActivity
{
    const int defaultTimeout = 30000;
    private static readonly ICaptchaAuth captchaAuthApi = RestService.For<ICaptchaAuth>(Resources.SolveCaptcha_BaseAuthUrl);
    private static readonly ICaptchaSolver captchaSolverApi = RestService.For<ICaptchaSolver>(Resources.SolveCaptcha_BaseApiUrl);

    [LocalizedCategory(nameof(Resources.Common_Category))]
    [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
    [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
    public override InArgument<bool> ContinueOnError { get; set; }

    [LocalizedDisplayName(nameof(Resources.Timeout_DisplayName))]
    [LocalizedDescription(nameof(Resources.Timeout_Description))]
    [LocalizedCategory(nameof(Resources.Common_Category))]
    public InArgument<int> TimeoutInput { get; set; }

    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.SolveCaptcha_CaptchaType_DisplayName))]
    [LocalizedDescription(nameof(Resources.SolveCaptcha_CaptchaType_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]

    public CaptchaTypes CaptchaType { get; set; }

    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.SolveCaptcha_Username_DisplayName))]
    [LocalizedDescription(nameof(Resources.SolveCaptcha_Username_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> Username { get; set; }

    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.SolveCaptcha_Password_DisplayName))]
    [LocalizedDescription(nameof(Resources.SolveCaptcha_Password_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<SecureString> Password { get; set; }

    [LocalizedDisplayName(nameof(Resources.SolveCaptcha_SiteUrl_DisplayName))]
    [LocalizedDescription(nameof(Resources.SolveCaptcha_SiteUrl_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> SiteUrl { get; set; }

    [LocalizedDisplayName(nameof(Resources.SolveCaptcha_CaptchaKey_DisplayName))]
    [LocalizedDescription(nameof(Resources.SolveCaptcha_CaptchaKey_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> CaptchaKey { get; set; }

    [LocalizedDisplayName(nameof(Resources.SolveCaptcha_ImageUrl_DisplayName))]
    [LocalizedDescription(nameof(Resources.SolveCaptcha_ImageUrl_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> ImageUrl { get; set; }

    [LocalizedDisplayName(nameof(Resources.SolveCaptcha_ResolutionToken_DisplayName))]
    [LocalizedDescription(nameof(Resources.SolveCaptcha_ResolutionToken_Description))]
    [LocalizedCategory(nameof(Resources.Output_Category))]
    public OutArgument<string> ResolutionToken { get; set; }

    protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
    {
        var timeoutInput = TimeoutInput.Get(context);
        var task = ExecuteWithTimeout(context);

        return await Task.WhenAny(task, Task.Delay(timeoutInput == 0 ? defaultTimeout : timeoutInput * 1000, cancellationToken)) != task
            ? throw new TimeoutException(Resources.Timeout_Error)
            : ((context) => ResolutionToken.Set(context, task.Result));
    }

    private async Task<string> ExecuteWithTimeout(AsyncCodeActivityContext context)
    {
        var imageUrl = ImageUrl.Get(context);
        var captchaKey = CaptchaKey.Get(context);
        var siteUrl = SiteUrl.Get(context);
        var acessToken = await GetAcessToken(Username.Get(context), Password.Get(context));

        var resolutionId = CaptchaType switch
        {
            CaptchaTypes.TextCaptcha => await SolveTextCaptcha(imageUrl, acessToken),
            CaptchaTypes.ReCaptcha => await SolveReCaptcha(captchaKey, siteUrl, acessToken),
            CaptchaTypes.HCaptcha => await SolveHCaptcha(captchaKey, siteUrl, acessToken),
            _ => throw new NotImplementedException("CapthaType Invalid")
        };
        var resolutionToken = await GetResolutionToken(resolutionId, acessToken);
        return resolutionToken;
    }

    private async Task<string> GetAcessToken(string username, SecureString password)
    {

        var bodyDict = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password.ToPlainString()},
            { "grant_type", Resources.SolveCaptcha_GrantType },
            { "client_id", Resources.SolveCaptcha_ClientId },
        };
        var bodyRequest = new FormUrlEncodedContent(bodyDict);
        var response = await captchaAuthApi.GetCaptchaAccessToken(bodyRequest);
        var accessToken = response.StatusCode switch
        {
            HttpStatusCode.OK => JsonNode.Parse(response.Content)["access_token"].ToString(),
            HttpStatusCode.Unauthorized => throw new InvalidCredentialsException("Invalid credentials. Please contact Smarthis Support at https://smarthis.com/contato/."),
            HttpStatusCode.InternalServerError => throw new ServiceUnavailableException("Authentication Service is currently unavailable. Please try again later."),
            _ => throw new NotImplementedException($"Unhandled error at authentication: {response.Content}. StatusCode: {response.StatusCode}")
        } ;
        return accessToken;
    }

    private async Task<string> GetResolutionToken(string resolutionId, string acessToken)
    {
        var resolutionToken = string.Empty;
        while (string.IsNullOrEmpty(resolutionToken))
        {
            var response = await captchaSolverApi.GetCaptchaResolutionToken(resolutionId, acessToken);
            await Task.Delay(2000);
            resolutionToken = response.StatusCode switch
            {
                HttpStatusCode.OK => response.Content,
                HttpStatusCode.Processing => null,
                (HttpStatusCode) 425 => null,
                HttpStatusCode.NotAcceptable => throw new FailToSolveCaptchaException("The service was unable to solve the captcha"),
                HttpStatusCode.InternalServerError => throw new ServiceUnavailableException("Retrieve information service is currently unavailable. Please try again later."),
                _ => throw new NotImplementedException($"Unhandled error retrieving information: {response.Content}. StatusCode: {response.StatusCode}")
            };
        }
        return resolutionToken;
    }

    private async Task<string> ValidateStatusCode(Func<Task<ApiResponse<string>>> solveFunction)
    {

        var response = await solveFunction();

        var resolutionId = response.StatusCode switch
        {
            HttpStatusCode.OK => response.Content,
            HttpStatusCode.Accepted => response.Content,
            HttpStatusCode.NoContent => throw new InvalidKeyException("Invalid key detected"),
            HttpStatusCode.InternalServerError => throw new ServiceUnavailableException("Send information service is currently unavailable. Please try again later."),
            _ => throw new NotImplementedException($"Unhandled error sending information: {response.Content}. StatusCode: {response.StatusCode}")
        };
        return resolutionId;

    }

    private async Task<string> SolveHCaptcha(string CaptchaKey, string SiteUrl, string acessToken)
    {
        var data = new Dictionary<string, string>()
    {
        { "siteKey", CaptchaKey },
        { "pageUrl", SiteUrl },
    };

        return await ValidateStatusCode(() => captchaSolverApi.GetHcaptchaResolutionId(data, acessToken));
    }

    private async Task<string> SolveReCaptcha(string CaptchaKey, string SiteUrl, string acessToken)
    {
        var data = new Dictionary<string, string>()
    {
        { "googleKey", CaptchaKey },
        { "pageUrl", SiteUrl },
    };

        return await ValidateStatusCode(() => captchaSolverApi.GetRecaptchaResolutionId(data, acessToken));
    }

    private async Task<string> SolveTextCaptcha(string imageUrl, string accessToken)
    {
        var httpClient = new HttpClient();
        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
        var captcha = new ByteArrayPart(imageBytes, "captcha");
        return await ValidateStatusCode(() => captchaSolverApi.GetTextcaptchaResolutionId(captcha, accessToken));
    }

}