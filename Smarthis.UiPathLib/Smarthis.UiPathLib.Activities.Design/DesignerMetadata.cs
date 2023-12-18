using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Smarthis.UiPathLib.Activities.Design.Designers;
using Smarthis.UiPathLib.Activities.Design.Properties;

namespace Smarthis.UiPathLib.Activities.Design;

/// <summary>
/// Adds the activities contained in this package to the list of activities in UiPath.
/// </summary>
public class DesignerMetadata : IRegisterMetadata
{
    /// <summary>
    /// Registers this package of activities in UiPath.
    /// </summary>
    public void Register()
    {
        var builder = new AttributeTableBuilder();
        builder.ValidateTable();

        var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

        #region NOTE: You need to add 3 lines using the following convention for every activity that you add to the project.
        
        builder.AddCustomAttributes(typeof(StringToSecureString), categoryAttribute);
        builder.AddCustomAttributes(typeof(StringToSecureString), new DesignerAttribute(typeof(StringToSecureStringDesigner)));
        builder.AddCustomAttributes(typeof(StringToSecureString), new HelpKeywordAttribute(""));
        
        builder.AddCustomAttributes(typeof(SecureStringToString), categoryAttribute);
        builder.AddCustomAttributes(typeof(SecureStringToString), new DesignerAttribute(typeof(SecureStringToStringDesigner)));
        builder.AddCustomAttributes(typeof(SecureStringToString), new HelpKeywordAttribute(""));

        builder.AddCustomAttributes(typeof(SolveCaptcha), categoryAttribute);
        builder.AddCustomAttributes(typeof(SolveCaptcha), new DesignerAttribute(typeof(SolveCaptchaDesigner)));
        builder.AddCustomAttributes(typeof(SolveCaptcha), new HelpKeywordAttribute(""));
        #endregion

        MetadataStore.AddAttributeTable(builder.CreateTable());
    }
}
