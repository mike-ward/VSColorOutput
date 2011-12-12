// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System.ComponentModel.Composition;
using BlueOnionSoftware.Properties;
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
        public const string LogSpecial = "LogSpecial";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(BuildHead)]
        public static ClassificationTypeDefinition BuildHeaderDefinition { get; set; }

        [Name(BuildHead)]
        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = BuildHead)]
        public sealed class BuildHeaderFormat : ClassificationFormatDefinition
        {
            public BuildHeaderFormat()
            {
                DisplayName = VsColorOut + "Build Header";
                ForegroundColor = Settings.Default.BuildHeaderForeground;
            }
        }

        [Export]
        [Name(BuildText)]
        public static ClassificationTypeDefinition BuildTextDefinition { get; set; }

        [Name(BuildText)]
        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = BuildText)]
        public sealed class BuildTextFormat : ClassificationFormatDefinition
        {
            public BuildTextFormat()
            {
                DisplayName = VsColorOut + "Build Text";
                ForegroundColor = Settings.Default.BuildTextForeground;
            }
        }

        [Export]
        [Name(LogSpecial)]
        public static ClassificationTypeDefinition LogSpecialDefinition { get; set; }

        [Name(LogSpecial)]
        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogSpecial)]
        public sealed class LogSpecialFormat : ClassificationFormatDefinition
        {
            public LogSpecialFormat()
            {
                DisplayName = VsColorOut + "Log Special";
                ForegroundColor = Settings.Default.LogSpecialForeground;
            }
        }

        [Export]
        [Name(LogError)]
        public static ClassificationTypeDefinition LogErrorDefinition { get; set; }

        [Name(LogError)]
        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogError)]
        public sealed class LogErrorFormat : ClassificationFormatDefinition
        {
            public LogErrorFormat()
            {
                DisplayName = VsColorOut + "Log Error";
                ForegroundColor = Settings.Default.LogErrorForeground;
            }
        }

        [Export]
        [Name(LogWarn)]
        public static ClassificationTypeDefinition LogWarningDefinition { get; set; }

        [Name(LogWarn)]
        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogWarn)]
        public sealed class LogWarningFormat : ClassificationFormatDefinition
        {
            public LogWarningFormat()
            {
                DisplayName = VsColorOut + "Log Warning";
                ForegroundColor = Settings.Default.LogWarningForeground;
            }
        }

        [Export]
        [Name(LogInfo)]
        public static ClassificationTypeDefinition LogInformationDefinition { get; set; }

        [Name(LogInfo)]
        [UserVisible(true)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = LogInfo)]
        public sealed class LogInformationFormat : ClassificationFormatDefinition
        {
            public LogInformationFormat()
            {
                DisplayName = VsColorOut + "Log Information";
                ForegroundColor = Settings.Default.LogInformationForeground;
            }
        }
    }
}