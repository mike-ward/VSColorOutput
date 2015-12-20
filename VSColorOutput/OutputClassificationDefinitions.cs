using System.ComponentModel.Composition;
using System.Windows.Media;
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

        [Export(typeof (ClassificationTypeDefinition))]
        [Name(BuildHead)]
        public static ClassificationTypeDefinition BuildHeaderDefinition { get; set; }

        [Name(BuildHead)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = BuildHead)]
        [Order(Before = Priority.Default)]
        public sealed class BuildHeaderFormat : ClassificationFormatDefinition
        {
            public BuildHeaderFormat()
            {
                DisplayName = VsColorOut + "Build Header";
                ForegroundColor = Colors.Green;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(BuildText)]
        public static ClassificationTypeDefinition BuildTextDefinition { get; set; }

        [Name(BuildText)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = BuildText)]
        [Order(Before = Priority.Default)]
        public sealed class BuildTextFormat : ClassificationFormatDefinition
        {
            public BuildTextFormat()
            {
                DisplayName = VsColorOut + "Build Text";
                ForegroundColor = Colors.Gray;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogError)]
        public static ClassificationTypeDefinition LogErrorDefinition { get; set; }

        [Name(LogError)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogError)]
        [Order(Before = Priority.Default)]
        public sealed class LogErrorFormat : ClassificationFormatDefinition
        {
            public LogErrorFormat()
            {
                DisplayName = VsColorOut + "Log Error";
                ForegroundColor = Colors.Red;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogWarn)]
        public static ClassificationTypeDefinition LogWarningDefinition { get; set; }

        [Name(LogWarn)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogWarn)]
        [Order(Before = Priority.Default)]
        public sealed class LogWarningFormat : ClassificationFormatDefinition
        {
            public LogWarningFormat()
            {
                DisplayName = VsColorOut + "Log Warning";
                ForegroundColor = Colors.Olive;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogInfo)]
        public static ClassificationTypeDefinition LogInformationDefinition { get; set; }

        [Name(LogInfo)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogInfo)]
        [Order(Before = Priority.Default)]
        public sealed class LogInformationFormat : ClassificationFormatDefinition
        {
            public LogInformationFormat()
            {
                DisplayName = VsColorOut + "Log Information";
                ForegroundColor = Colors.DarkBlue;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogCustom1)]
        public static ClassificationTypeDefinition LogCustome1Definition { get; set; }

        [Name(LogCustom1)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogCustom1)]
        [Order(Before = Priority.Default)]
        public sealed class LogCustom1Format : ClassificationFormatDefinition
        {
            public LogCustom1Format()
            {
                DisplayName = VsColorOut + "Log Custom1";
                ForegroundColor = Colors.Purple;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogCustom2)]
        public static ClassificationTypeDefinition LogCustom2Definition { get; set; }

        [Name(LogCustom2)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogCustom2)]
        [Order(Before = Priority.Default)]
        public sealed class LogCustom2Format : ClassificationFormatDefinition
        {
            public LogCustom2Format()
            {
                DisplayName = VsColorOut + "Log Custom2";
                ForegroundColor = Colors.DarkSalmon;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogCustom3)]
        public static ClassificationTypeDefinition LogCustom3Definition { get; set; }

        [Name(LogCustom3)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogCustom3)]
        [Order(Before = Priority.Default)]
        public sealed class LogCustom3Format : ClassificationFormatDefinition
        {
            public LogCustom3Format()
            {
                DisplayName = VsColorOut + "Log Custom3";
                ForegroundColor = Colors.DarkOrange;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(LogCustom4)]
        public static ClassificationTypeDefinition LogCustom4Definition { get; set; }

        [Name(LogCustom4)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogCustom4)]
        [Order(Before = Priority.Default)]
        public sealed class LogCustom4Format : ClassificationFormatDefinition
        {
            public LogCustom4Format()
            {
                DisplayName = VsColorOut + "Log Custom4";
                ForegroundColor = Colors.Brown;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(FindResultsSearchTerm)]
        public static ClassificationTypeDefinition FindResultsSearchTermDefinition { get; set; }

        [Name(FindResultsSearchTerm)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = FindResultsSearchTerm)]
        [Order(Before = Priority.Default)]
        public sealed class FindResultsSearchTermFormat : ClassificationFormatDefinition
        {
            public FindResultsSearchTermFormat()
            {
                DisplayName = VsColorOut + "Find Results Search Term";
                ForegroundColor = Colors.Green;
                BackgroundOpacity = 0;
            }
        }

        [Export]
        [Name(FindResultsFilename)]
        public static ClassificationTypeDefinition FindFilenameDefinition { get; set; }

        [Name(FindResultsFilename)]
        [UserVisible(true)]
        [Export(typeof (EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = FindResultsFilename)]
        [Order(Before = Priority.Default)]
        public sealed class FindResultsFilenameFormat : ClassificationFormatDefinition
        {
            public FindResultsFilenameFormat()
            {
                DisplayName = VsColorOut + "Find Results Filename";
                ForegroundColor = Colors.Gray;
                BackgroundOpacity = 0;
            }
        }
    }
}