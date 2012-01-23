// Copyright (c) 2012 Blue Onion Software, All rights reserved
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace BlueOnionSoftware
{
    [Guid("BE905985-26BB-492B-9453-743E26F4E8BB")]
    public class VsColorOutputOptions : DialogPage
    {
        private const string ActionSubCategory = "Actions";
        private const string PatternsSubCategory = "Patterns";
        public const string Category = "VSColorOutput";
        public const string SubCategory = "General";

        [Category(PatternsSubCategory)]
        [DisplayName("RegEx Patterns")]
        [Description(
            "Specify patterns (Regular Expressions) and assoicate with classification types. " +
            "The order of the patterns is significant. " +
            "Delete all patterns to restore default patterns.")]
        public RegExClassification[] RegExPatterns { get; set; }

        [Category(ActionSubCategory)]
        [DisplayName("Stop Build on First Error")]
        [Description("Stops the build on the first project error")]
        public bool StopOnFirstBuildError { get; set; }

        [Category(ActionSubCategory)]
        [DisplayName("Show Elapsed Build Time")]
        [Description("Shows the elapsed build time when the build finishes")]
        public bool ShowElapsedBuildTime { get; set; }

        [Category(ActionSubCategory)]
        [DisplayName("Show Debug Window on Debug")]
        [Description("Shows the debug window when the debug session starts")]
        public bool ShowDebugWindowOnDebug { get; set; }

        public override void LoadSettingsFromStorage()
        {
            var settings = new Settings();
            settings.Load();
            RegExPatterns = settings.Patterns;
            StopOnFirstBuildError = settings.EnableStopOnBuildError;
            ShowElapsedBuildTime = settings.ShowElapsedBuildTime;
            ShowDebugWindowOnDebug = settings.ShowDebugWindowOnDebug;
        }

        public override void SaveSettingsToStorage()
        {
            var settings = new Settings
            {
                Patterns = RegExPatterns, 
                EnableStopOnBuildError = StopOnFirstBuildError,
                ShowElapsedBuildTime = ShowElapsedBuildTime,
                ShowDebugWindowOnDebug = ShowDebugWindowOnDebug
            };
            settings.Save();
        }
    }
}