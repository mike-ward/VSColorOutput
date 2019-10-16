using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace VSColorOutput.Output.ColorClassifier
{
    public static class ClassificationTypeDefinitions
    {
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

        public const string TimeStamp = "TimeStamp";

        private static bool _settingsLoaded;
        private static Dictionary<string, Color> _colorMap;

        private static System.Windows.Media.Color GetColor(string classificationName)
        {
            if (_settingsLoaded == false)
            {
                _colorMap = ColorMap.GetMap();
                _settingsLoaded = true;
            }
            
            if (!_colorMap.TryGetValue(classificationName, out Color color)) color = Color.Gray;
            return ToMediaColor(color);
        }

        public static System.Windows.Media.Color ToMediaColor(Color color)
        {
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
                ForegroundColor = GetColor(BuildHead);
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
                ForegroundColor = GetColor(BuildText);
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
                ForegroundColor = GetColor(LogError);
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
                ForegroundColor = GetColor(LogWarn);
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
                ForegroundColor = GetColor(LogInfo);
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
                ForegroundColor = GetColor(LogCustom1);
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
                ForegroundColor = GetColor(LogCustom2);
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
                ForegroundColor = GetColor(LogCustom3);
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
                ForegroundColor = GetColor(LogCustom4);
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
                ForegroundColor = GetColor(FindResultsSearchTerm);
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
                ForegroundColor = GetColor(FindResultsFilename);
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(TimeStamp)]
        public static ClassificationTypeDefinition TimeStampDefinition { get; set; }

        [Name(TimeStamp)]
        [UserVisible(false)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = TimeStamp)]
        [Order(Before = Priority.Default)]
        public sealed class TimeStampFormat : ClassificationFormatDefinition
        {
            public TimeStampFormat()
            {
                ForegroundColor = GetColor(TimeStamp);
                BackgroundOpacity = 0;
            }
        }
    }
}