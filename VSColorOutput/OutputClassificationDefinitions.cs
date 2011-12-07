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

        public const string BuildHead = VsColorOut + "Build Header";
        public const string BuildText = VsColorOut + "Build Text";

        public const string LogInfo = VsColorOut + "Log Information";
        public const string LogWarn = VsColorOut + "Log Warning";
        public const string LogError = VsColorOut + "Log Error";
        public const string LogSpecial = VsColorOut + "Log Special";

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
                DisplayName = BuildHead;
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
                DisplayName = BuildText;
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
                DisplayName = LogSpecial;
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
                DisplayName = LogError;
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
                DisplayName = LogWarn;
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
                DisplayName = LogInfo;
                ForegroundColor = Settings.Default.LogInformationForeground;
            }
        }
    }
}