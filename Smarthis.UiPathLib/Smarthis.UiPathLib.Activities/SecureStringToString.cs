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

[LocalizedDisplayName(nameof(Resources.SecureStringToString_DisplayName))]
[LocalizedDescription(nameof(Resources.SecureStringToString_Description))]
public class SecureStringToString : ContinuableAsyncCodeActivity
{
    [LocalizedCategory(nameof(Resources.Common_Category))]
    [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
    [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
    public override InArgument<bool> ContinueOnError { get; set; }
    
    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.SecureStringToString_SecuredString_DisplayName))]
    [LocalizedDescription(nameof(Resources.SecureStringToString_SecuredString_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<SecureString> SecuredString { get; set; }
    
    [LocalizedDisplayName(nameof(Resources.SecureStringToString_UnsecuredString_DisplayName))]
    [LocalizedDescription(nameof(Resources.SecureStringToString_UnsecuredString_Description))]
    [LocalizedCategory(nameof(Resources.Output_Category))]
    public OutArgument<string> UnsecuredString { get; set; }

    protected override void CacheMetadata(CodeActivityMetadata metadata)
    {
        if (SecuredString == null)
            metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(SecuredString)));
        base.CacheMetadata(metadata);
    }
    
    protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken) 
        => ctx => UnsecuredString.Set(ctx, SecuredString.Get(ctx).ToPlainString());
}