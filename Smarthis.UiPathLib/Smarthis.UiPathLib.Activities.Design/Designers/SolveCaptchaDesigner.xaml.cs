using Smarthis.UiPathLib.Activities.Modules.SolveCaptcha.Enum;
using System.Collections.Generic;
using System;
using UiPath.Shared.Activities.Localization;
using System.Linq;

namespace Smarthis.UiPathLib.Activities.Design.Designers
{
    public partial class SolveCaptchaDesigner
    {
        internal static IEnumerable<CaptchaTypes> ComboSelectionEnums => Enum.GetValues(typeof(CaptchaTypes)).Cast<CaptchaTypes>();

        public SolveCaptchaDesigner()
        {
            InitializeComponent();

        }
    }
}
