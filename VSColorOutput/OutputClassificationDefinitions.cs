using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BlueOnionSoftware
{
    public static class OutputClassificationDefinitions
    {
        private const string VsColorOut = "VSColorOutput ";

        public const string BuildHead = "BuildHead";
        public const string BuildText = "BuildText";

        public const string LogInfo = "LogInformation";
        public const string LogWarn = "LogWarning";
        public const string LogError = "LogError";
        public const string LogCustom1 = "LogCustom1";
        public const string LogCustom2 = "LogCustom2";
        public const string LogCustom3 = "LogCustom3";
        public const string LogCustom4 = "LogCustom4";

        public const string FindResultsSearchTerm = "FindResultsSearchTerm";
        public const string FindResultsFilename = "FindResultsFilename";

        private static bool _settingsLoaded;
        private static Dictionary<ClassificationTypes, Color> _colorMap;

        static OutputClassificationDefinitions()
        {
            Settings.SettingsUpdated += (sender, args) => _settingsLoaded = false;
        }

        private static System.Windows.Media.Color GetColor(ClassificationTypes classificationType)
        {
            if (_settingsLoaded == false)
            {
                var settings = Settings.Load();
                _colorMap = new Dictionary<ClassificationTypes, Color>
                {
                    {ClassificationTypes.BuildHead, settings.BuildMessageColor},
                    {ClassificationTypes.BuildText, settings.BuildTextColor},
                    {ClassificationTypes.LogError, settings.ErrorTextColor},
                    {ClassificationTypes.LogWarning, settings.WarningTextColor},
                    {ClassificationTypes.LogInformation, settings.InformationTextColor},
                    {ClassificationTypes.LogCustom1, settings.CustomTextColor1},
                    {ClassificationTypes.LogCustom2, settings.CustomTextColor2},
                    {ClassificationTypes.LogCustom3, settings.CustomTextColor3},
                    {ClassificationTypes.LogCustom4, settings.CustomTextColor4},
                    {ClassificationTypes.FindFileName, settings.FindFileNameColor},
                    {ClassificationTypes.FindSearchTerm, settings.FindSearchTermColor}
                };
                _settingsLoaded = true;
            }
            Color color;
            if (!_colorMap.TryGetValue(classificationType, out color)) color = Color.Gray;
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(BuildHead)]
        public static ClassificationTypeDefinition BuildHeaderDefinition { get; set; }

        [Name(BuildHead)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = BuildHead)]
        [Order(Before = Priority.Default)]
        public sealed class BuildHeaderFormat : ClassificationFormatDefinition
        {
            public BuildHeaderFormat()
            {
                DisplayName = VsColorOut + "Build Header";
                ForegroundColor = GetColor(ClassificationTypes.BuildHead);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(BuildText)]
        public static ClassificationTypeDefinition BuildTextDefinition { get; set; }

        [Name(BuildText)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = BuildText)]
        [Order(Before = Priority.Default)]
        public sealed class BuildTextFormat : ClassificationFormatDefinition
        {
            public BuildTextFormat()
            {
                DisplayName = VsColorOut + "Build Text";
                ForegroundColor = GetColor(ClassificationTypes.BuildText);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogError)]
        public static ClassificationTypeDefinition LogErrorDefinition { get; set; }

        [Name(LogError)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogError)]
        [Order(Before = Priority.Default)]
        public sealed class LogErrorFormat : ClassificationFormatDefinition
        {
            public LogErrorFormat()
            {
                DisplayName = VsColorOut + "Log Error";
                ForegroundColor = GetColor(ClassificationTypes.LogError);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogWarn)]
        public static ClassificationTypeDefinition LogWarningDefinition { get; set; }

        [Name(LogWarn)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogWarn)]
        [Order(Before = Priority.Default)]
        public sealed class LogWarningFormat : ClassificationFormatDefinition
        {
            public LogWarningFormat()
            {
                DisplayName = VsColorOut + "Log Warning";
                ForegroundColor = GetColor(ClassificationTypes.LogWarning);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogInfo)]
        public static ClassificationTypeDefinition LogInformationDefinition { get; set; }

        [Name(LogInfo)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogInfo)]
        [Order(Before = Priority.Default)]
        public sealed class LogInformationFormat : ClassificationFormatDefinition
        {
            public LogInformationFormat()
            {
                DisplayName = VsColorOut + "Log Information";
                ForegroundColor = GetColor(ClassificationTypes.LogInformation);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogCustom1)]
        public static ClassificationTypeDefinition LogCustome1Definition { get; set; }

        [Name(LogCustom1)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogCustom1)]
        [Order(Before = Priority.Default)]
        public sealed class LogCustom1Format : ClassificationFormatDefinition
        {
            public LogCustom1Format()
            {
                DisplayName = VsColorOut + "Log Custom1";
                ForegroundColor = GetColor(ClassificationTypes.LogCustom1);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogCustom2)]
        public static ClassificationTypeDefinition LogCustom2Definition { get; set; }

        [Name(LogCustom2)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogCustom2)]
        [Order(Before = Priority.Default)]
        public sealed class LogCustom2Format : ClassificationFormatDefinition
        {
            public LogCustom2Format()
            {
                DisplayName = VsColorOut + "Log Custom2";
                ForegroundColor = GetColor(ClassificationTypes.LogCustom2);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogCustom3)]
        public static ClassificationTypeDefinition LogCustom3Definition { get; set; }

        [Name(LogCustom3)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogCustom3)]
        [Order(Before = Priority.Default)]
        public sealed class LogCustom3Format : ClassificationFormatDefinition
        {
            public LogCustom3Format()
            {
                DisplayName = VsColorOut + "Log Custom3";
                ForegroundColor = GetColor(ClassificationTypes.LogCustom3);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogCustom4)]
        public static ClassificationTypeDefinition LogCustom4Definition { get; set; }

        [Name(LogCustom4)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogCustom4)]
        [Order(Before = Priority.Default)]
        public sealed class LogCustom4Format : ClassificationFormatDefinition
        {
            public LogCustom4Format()
            {
                DisplayName = VsColorOut + "Log Custom4";
                ForegroundColor = GetColor(ClassificationTypes.LogCustom4);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(FindResultsSearchTerm)]
        public static ClassificationTypeDefinition FindResultsSearchTermDefinition { get; set; }

        [Name(FindResultsSearchTerm)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = FindResultsSearchTerm)]
        [Order(Before = Priority.Default)]
        public sealed class FindResultsSearchTermFormat : ClassificationFormatDefinition
        {
            public FindResultsSearchTermFormat()
            {
                DisplayName = VsColorOut + "Find Results Search Term";
                ForegroundColor = GetColor(ClassificationTypes.FindSearchTerm);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(FindResultsFilename)]
        public static ClassificationTypeDefinition FindFilenameDefinition { get; set; }

        [Name(FindResultsFilename)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = FindResultsFilename)]
        [Order(Before = Priority.Default)]
        public sealed class FindResultsFilenameFormat : ClassificationFormatDefinition
        {
            public FindResultsFilenameFormat()
            {
                DisplayName = VsColorOut + "Find Results Filename";
                ForegroundColor = GetColor(ClassificationTypes.FindFileName);
                BackgroundOpacity = 0;
            }
        }
    }
}