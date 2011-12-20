// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace BlueOnionSoftware
{
    [Guid("BE905985-26BB-492B-9453-743E26F4E8BB")]
    public class VsColorOutputOptions : DialogPage
    {
        public const string Category = "VSColorOutput";
        public const string SubCategory = "General";

        [DisplayName("RegEx Patterns")]
        [Description("Specify patterns (Regular Expressions) and assoicate with classification types. " +
                     "The order of the patterns is significant. " +
                     "Delete all patterns to restore default patterns.")]
        public RegExClassification[] RegExPatterns { get; set; }

        public override void LoadSettingsFromStorage()
        {
            RegExPatterns = Settings.LoadPatterns();
        }

        public override void SaveSettingsToStorage()
        {
            Settings.SaveSettingsToStorage(RegExPatterns);
        }
    }
}