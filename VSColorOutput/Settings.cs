using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Media;

namespace BlueOnionSoftware
{
    public class Settings
    {
        public const string RegistryPath = @"DialogPage\BlueOnionSoftware.VsColorOutputOptions";

        public bool EnableStopOnBuildError { get; set; }
        public bool ShowElapsedBuildTime { get; set; }
        public bool ShowBuildReport { get; set; }
        public bool ShowDebugWindowOnDebug { get; set; }
        public bool HighlightFindResults { get; set; }

        public RegExClassification[] Patterns { get; set; } = DefaultPatterns();

        public Color BuildMessageColor { get; set; } = Colors.Green;
        public Color BuildTextColor { get; set; } = Colors.Gray;

        public Color ErrorTextColor { get; set; } = Colors.Red;
        public Color WarningTextColor { get; set; } = Colors.Olive;
        public Color InformationTextColor { get; set; } = Colors.DarkBlue;

        public Color CustomTextColor1 { get; set; } = Colors.Purple;
        public Color CustomTextColor2 { get; set; } = Colors.DarkSalmon;
        public Color CustomTextColor3 { get; set; } = Colors.DarkOrange;
        public Color CustomTextColor4 { get; set; } = Colors.Brown;

        public Color FindSearchTermColor { get; set; } = Colors.Green;
        public Color FindFileNameColor { get; set; } = Colors.Gray;

        private static readonly string AppDataFolder;
        private static readonly string SettingsFile;

        public static event EventHandler SettingsChanged;

        private static void OnSettingsChanged(object sender, EventArgs ea) => SettingsChanged?.Invoke(sender, ea);

        static Settings()
        {
            AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            SettingsFile = Path.Combine(AppDataFolder, "VSColorOutput/settings.json");
        }

        public static Settings Load()
        {
            if (!File.Exists(SettingsFile)) new Settings().Save();
            using (var stream = new FileStream(SettingsFile, FileMode.Open))
            {
                var deserialize = new DataContractJsonSerializer(typeof(Settings));
                return (Settings)deserialize.ReadObject(stream);
            }
        }

        public void Save()
        {
            Directory.CreateDirectory(AppDataFolder);
            using (var stream = new FileStream(SettingsFile, FileMode.Create))
            {
                var serializer = new DataContractJsonSerializer(typeof(Settings));
                serializer.WriteObject(stream, this);
            }
            OnSettingsChanged(this, EventArgs.Empty);
        }

        private static RegExClassification[] DefaultPatterns()
        {
            return new[]
            {
                new RegExClassification {RegExPattern = @"\+\+\+\>", ClassificationType = ClassificationTypes.LogCustom1, IgnoreCase = false},
                new RegExClassification {RegExPattern = @"(=====|-----|Projects build report|Status    \| Project \[Config\|platform\])", ClassificationType = ClassificationTypes.BuildHead, IgnoreCase = false},
                new RegExClassification {RegExPattern = @"0 failed|Succeeded", ClassificationType = ClassificationTypes.BuildHead, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(\W|^)(error|fail|failed|exception)[^\w\.]", ClassificationType = ClassificationTypes.LogError, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(exception:|stack trace:)", ClassificationType = ClassificationTypes.LogError, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"^\s+at\s", ClassificationType = ClassificationTypes.LogError, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(\W|^)warning\W", ClassificationType = ClassificationTypes.LogWarning, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(\W|^)information\W", ClassificationType = ClassificationTypes.LogInformation, IgnoreCase = true}
            };
        }
    }
}