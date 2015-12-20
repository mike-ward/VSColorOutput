using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;

namespace BlueOnionSoftware
{
    [Guid("BE905985-26BB-492B-9453-743E26F4E8BB")]
    public class VsColorOutputOptions : DialogPage
    {
        private const string ActionSubCategory = "Build Actions";
        private const string PatternsSubCategory = "Classifier Patterns";
        private const string ColorsSubCategory = "Colors (Requires restart)";
        public const string Category = "VSColorOutput";
        public const string SubCategory = "General";

        [Category(PatternsSubCategory)]
        [DisplayName("RegEx Patterns")]
        [Description("Specify patterns (Regular Expressions) and assoicate with classification types. " +
            "The order of the patterns is significant. ")]
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
        [DisplayName("Show Build Report")]
        [Description("Shows the list of projects built at the end of the build")]
        public bool ShowBuildReport { get; set; }

        [Category(ActionSubCategory)]
        [DisplayName("Show Debug Window on Debug")]
        [Description("Shows the debug window when the debug session starts")]
        public bool ShowDebugWindowOnDebug { get; set; }

        [Category(ActionSubCategory)]
        [DisplayName("Color Find Results")]
        public bool HighlightFindResults { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Build Message Color")]
        [Description("Used for messages (they usually start with =====)")]
        public Color BuildMessageColor { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Build Text Color")]
        [Description("Other build text")]
        public Color BuildTextColor { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Error Text Color")]
        public Color ErrorTextColor { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Warning Text Color")]
        public Color WarningTextColor { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Information Text Color")]
        public Color InformationTextColor { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Custom Text Color 1")]
        public Color CustomTextColor1 { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Custom Text Color 2")]
        public Color CustomTextColor2 { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Custom Text Color 3")]
        public Color CustomTextColor3 { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Custom Text Color 4")]
        public Color CustomTextColor4 { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Find File Name Color")]
        public Color FindFileNameColor { get; set; }

        [Category(ColorsSubCategory)]
        [DisplayName("Find Search Color")]
        public Color FindSearchTermColor { get; set; }

        public override void LoadSettingsFromStorage()
        {
            var settings = Settings.Load();

            StopOnFirstBuildError = settings.EnableStopOnBuildError;
            ShowElapsedBuildTime = settings.ShowElapsedBuildTime;
            ShowBuildReport = settings.ShowBuildReport;
            ShowDebugWindowOnDebug = settings.ShowDebugWindowOnDebug;
            HighlightFindResults = settings.HighlightFindResults;

            RegExPatterns = settings.Patterns;

            BuildMessageColor = settings.BuildMessageColor;
            BuildTextColor = settings.BuildTextColor;

            ErrorTextColor = settings.ErrorTextColor;
            WarningTextColor = settings.WarningTextColor;
            InformationTextColor = settings.InformationTextColor;

            CustomTextColor1 = settings.CustomTextColor1;
            CustomTextColor2 = settings.CustomTextColor2;
            CustomTextColor3 = settings.CustomTextColor3;
            CustomTextColor4 = settings.CustomTextColor4;

            FindFileNameColor = settings.FindFileNameColor;
            FindSearchTermColor = settings.FindSearchTermColor;
        }

        public override void SaveSettingsToStorage()
        {
            var settings = new Settings
            {
                EnableStopOnBuildError = StopOnFirstBuildError,
                ShowElapsedBuildTime = ShowElapsedBuildTime,
                ShowBuildReport = ShowBuildReport,
                ShowDebugWindowOnDebug = ShowDebugWindowOnDebug,
                HighlightFindResults = HighlightFindResults,
                // ---
                Patterns = RegExPatterns,
                // ---
                BuildMessageColor = BuildMessageColor,
                BuildTextColor = BuildTextColor,
                // ---
                ErrorTextColor = ErrorTextColor,
                WarningTextColor = WarningTextColor,
                InformationTextColor = InformationTextColor,
                // ---
                CustomTextColor1 = CustomTextColor1,
                CustomTextColor2 = CustomTextColor2,
                CustomTextColor3 = CustomTextColor3,
                CustomTextColor4 = CustomTextColor4,
                // --
                FindFileNameColor = FindFileNameColor,
                FindSearchTermColor = FindSearchTermColor
            };
            settings.Save();
        }
    }
}