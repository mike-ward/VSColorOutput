// Copyright (c) 2012 Blue Onion Software, All rights reserved
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

        [Category("Patterns")]
        [DisplayName("RegEx Patterns")]
        [Description(
            "Specify patterns (Regular Expressions) and assoicate with classification types. " +
            "The order of the patterns is significant. " +
            "Delete all patterns to restore default patterns.")]
        public RegExClassification[] RegExPatterns { get; set; }

        [Category("Actions")]
        [DisplayName("Stop Build on First Error")]
        [Description("Stops the build on the first project error")]
        public bool StopOnFirstBuildError { get; set; }

        [Category("Actions")]
        [DisplayName("Show Elapsed Build Time")]
        [Description("Shows the elapsed build time when the build finishes")]
        public bool ShowElapsedBuildTime { get; set; }

        public override void LoadSettingsFromStorage()
        {
            var settings = new Settings();
            settings.Load();
            RegExPatterns = settings.Patterns;
            StopOnFirstBuildError = settings.EnableStopOnBuildError;
            ShowElapsedBuildTime = settings.ShowElapsedBuildTime;
        }

        public override void SaveSettingsToStorage()
        {
            var settings = new Settings
            {
                Patterns = RegExPatterns, 
                EnableStopOnBuildError = StopOnFirstBuildError,
                ShowElapsedBuildTime = ShowElapsedBuildTime
            };
            settings.Save();
        }
    }
}