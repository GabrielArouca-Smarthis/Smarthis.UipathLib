using System;
using System.Activities;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Smarthis.UiPathLib.Activities.Extension;
using Smarthis.UiPathLib.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Smarthis.UiPathLib.Activities;

[LocalizedDisplayName(nameof(Resources.StringToSecureString_DisplayName))]
[LocalizedDescription(nameof(Resources.StringToSecureString_Description))]
public class StringToSecureString : ContinuableAsyncCodeActivity
{
    [LocalizedCategory(nameof(Resources.Common_Category))]
    [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
    [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
    public override InArgument<bool> ContinueOnError { get; set; }
    
    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.StringToSecureString_UnsecuredString_DisplayName))]
    [LocalizedDescription(nameof(Resources.StringToSecureString_UnsecuredString_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> UnsecuredString { get; set; }
    
    [LocalizedDisplayName(nameof(Resources.StringToSecureString_SecuredString_DisplayName))]
    [LocalizedDescription(nameof(Resources.StringToSecureString_SecuredString_Description))]
    [LocalizedCategory(nameof(Resources.Output_Category))]
    public OutArgument<SecureString> SecuredString { get; set; }
    
    protected override void CacheMetadata(CodeActivityMetadata metadata)
    {
        if (UnsecuredString == null)
            metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(UnsecuredString)));
        base.CacheMetadata(metadata);
    }

    protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken) 
        => ctx => SecuredString.Set(ctx, UnsecuredString.Get(ctx).ToSecureString());
}