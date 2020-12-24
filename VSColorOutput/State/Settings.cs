using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using VSColorOutput.Output.BuildEvents;
using VSColorOutput.Output.ColorClassifier;

namespace VSColorOutput.State
{
    [DataContract]
    public class Settings
    {
        public const string DefaultTimeStampFormat = "mm':'ss':'fff";
        public const string RegistryPath = @"DialogPage\BlueOnionSoftware.VsColorOutputOptions";

        [DataMember(Order = 0)]
        public bool EnableStopOnBuildError { get; set; }

        [DataMember(Order = 1)]
        public bool ShowElapsedBuildTime { get; set; }

        [DataMember(Order = 2)]
        public bool ShowBuildReport { get; set; }

        [DataMember(Order = 3)]
        public bool ShowDebugWindowOnDebug { get; set; }

        [DataMember(Order = 4)]
        public bool HighlightFindResults { get; set; }

        [DataMember(Order = 5)]
        public bool ShowTimeStamps { get; set; }

        [DataMember(Order = 6)]
        public bool ShowTimeStampOnEveryLine { get; set; }

        [DataMember(Order = 7)]
        public bool ShowHoursInTimeStamps { get; set; }

        [DataMember(Order = 8)]
        public bool FormatTimeInSystemLocale { get; set; }

        [DataMember(Order = 9)]
        public bool SuppressDonation { get; set; }

        [DataMember(Order = 10)]
        public RegExClassification[] Patterns { get; set; } = DefaultPatterns();

        [DataMember(Order = 11)]
        public Color BuildMessageColor { get; set; } = Color.Green;

        [DataMember(Order = 12)]
        public Color BuildTextColor { get; set; } = Color.Gray;

        [DataMember(Order = 13)]
        public Color ErrorTextColor { get; set; } = Color.Red;

        [DataMember(Order = 14)]
        public Color WarningTextColor { get; set; } = Color.Olive;

        [DataMember(Order = 15)]
        public Color InformationTextColor { get; set; } = Color.DarkBlue;

        [DataMember(Order = 16)]
        public Color CustomTextColor1 { get; set; } = Color.DarkOrange;

        [DataMember(Order = 17)]
        public Color CustomTextColor2 { get; set; } = Color.DarkSalmon;

        [DataMember(Order = 18)]
        public Color CustomTextColor3 { get; set; } = Color.Purple;

        [DataMember(Order = 19)]
        public Color CustomTextColor4 { get; set; } = Color.Brown;

        [DataMember(Order = 20)]
        public Color FindSearchTermColor { get; set; } = Color.Green;

        [DataMember(Order = 21)]
        public Color FindFileNameColor { get; set; } = Color.Gray;

        [DataMember(Order = 22)]
        public Color TimeStampColor { get; set; } = Color.CornflowerBlue;

        [DataMember(Order = 20)]
        public String TimeStampElapsed { get; set; } = DefaultTimeStampFormat;

        [DataMember(Order = 21)]
        public String TimeStampDifference { get; set; } = DefaultTimeStampFormat;

        private static readonly string ProgramDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VSColorOutput");

        public static event EventHandler SettingsUpdated;

        private static void OnSettingsUpdated(object sender, EventArgs ea) => SettingsUpdated?.Invoke(sender, ea);

        private static string GetSettingsFilePath()
        {
            const string name = "vscoloroutput.json";
            var settingsPath = Path.Combine(ProgramDataFolder, name);
            var solutionPath = BuildEvents.SolutionPath;

            if (solutionPath != null)
            {
                var path = Path.Combine(solutionPath, name);
                if (File.Exists(path))
                {
                    settingsPath = path;
                    if (string.IsNullOrWhiteSpace(File.ReadAllText(settingsPath)))
                    {
                        new Settings().SaveToFile(settingsPath);
                    }
                }
            }

            if (!File.Exists(settingsPath))
            {
                new Settings().SaveToFile(settingsPath);
            }

            return settingsPath;
        }

        public static Settings Load()
        {
            if (Runtime.RunningUnitTests) return new Settings();
            Directory.CreateDirectory(ProgramDataFolder);
            using (var stream = new FileStream(GetSettingsFilePath(), FileMode.Open))
            {
                var deserialize = new DataContractJsonSerializer(typeof(Settings));
                var settings = (Settings)deserialize.ReadObject(stream);
                // set missing colors
                if (settings.BuildMessageColor == Color.Empty) settings.BuildMessageColor = Color.Green;
                if (settings.BuildTextColor == Color.Empty) settings.BuildTextColor = Color.Gray;
                if (settings.ErrorTextColor == Color.Empty) settings.ErrorTextColor = Color.Red;
                if (settings.WarningTextColor == Color.Empty) settings.WarningTextColor = Color.Olive;
                if (settings.InformationTextColor == Color.Empty) settings.InformationTextColor = Color.DarkBlue;
                if (settings.CustomTextColor1 == Color.Empty) settings.CustomTextColor1 = Color.DarkOrange;
                if (settings.CustomTextColor2 == Color.Empty) settings.CustomTextColor2 = Color.DarkSalmon;
                if (settings.CustomTextColor3 == Color.Empty) settings.CustomTextColor3 = Color.Purple;
                if (settings.CustomTextColor4 == Color.Empty) settings.CustomTextColor4 = Color.Brown;
                if (settings.FindSearchTermColor == Color.Empty) settings.FindSearchTermColor = Color.Green;
                if (settings.FindFileNameColor == Color.Empty) settings.FindFileNameColor = Color.Gray;
                if (settings.TimeStampColor == Color.Empty) settings.TimeStampColor = Color.CornflowerBlue;
                return settings;
            }
        }

        public void Save()
        {
            if (Runtime.RunningUnitTests) return;
            Directory.CreateDirectory(ProgramDataFolder);
            SaveToFile(GetSettingsFilePath());
            OnSettingsUpdated(this, EventArgs.Empty);
        }

        public void SaveToFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var serializer = new DataContractJsonSerializer(typeof(Settings));
                serializer.WriteObject(stream, this);
            }
        }

        private static RegExClassification[] DefaultPatterns()
        {
            return new[]
            {
                new RegExClassification
                {
                    RegExPattern = @"\+\+\+\>",
                    ClassificationType = ClassificationTypes.LogCustom1,
                    IgnoreCase = false
                },
                new RegExClassification
                {
                    RegExPattern = @".*[t|c]sc\.exe.*",
                    ClassificationType = ClassificationTypes.BuildText,
                    IgnoreCase = false
                },
                new RegExClassification
                {
                    RegExPattern = @"(=====|-----|Projects build report|Status    \| Project \[Config\|platform\])",
                    ClassificationType = ClassificationTypes.BuildHead,
                    IgnoreCase = false
                },
                new RegExClassification
                {
                    RegExPattern = @"0 error.+0 warning",
                    ClassificationType = ClassificationTypes.BuildHead,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"^\s*0 error\(s\)\s*$",
                    ClassificationType = ClassificationTypes.BuildHead,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"^\s*0 warning\(s\)\s*$",
                    ClassificationType = ClassificationTypes.BuildHead,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"0 failed|Succeeded",
                    ClassificationType = ClassificationTypes.BuildHead,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"(\W|^)^(?!.*warning\s(BC|CS|CA)\d+:).*((?<!/)error|fail|failed|exception)[^\w\.\-\+]",
                    ClassificationType = ClassificationTypes.LogError,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"(exception:|stack trace:)",
                    ClassificationType = ClassificationTypes.LogError,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"^\s+at\s",
                    ClassificationType = ClassificationTypes.LogError,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"(\W|^)warning\W",
                    ClassificationType = ClassificationTypes.LogWarning,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"(\W|^)information\W",
                    ClassificationType = ClassificationTypes.LogInformation,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"Could not find file",
                    ClassificationType = ClassificationTypes.LogError,
                    IgnoreCase = true
                },
                new RegExClassification
                {
                    RegExPattern = @"failed", // .Net Core output
                    ClassificationType = ClassificationTypes.LogError,
                    IgnoreCase = true
                }
            };
        }
    }
}