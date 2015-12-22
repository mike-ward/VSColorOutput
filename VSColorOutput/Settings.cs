using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using static System.Environment;

namespace BlueOnionSoftware
{
    [DataContract]
    public class Settings
    {
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
        public RegExClassification[] Patterns { get; set; } = DefaultPatterns();

        [DataMember(Order = 6)]
        public Color BuildMessageColor { get; set; } = Color.Green;

        [DataMember(Order = 7)]
        public Color BuildTextColor { get; set; } = Color.Gray;

        [DataMember(Order = 8)]
        public Color ErrorTextColor { get; set; } = Color.Red;

        [DataMember(Order = 9)]
        public Color WarningTextColor { get; set; } = Color.Olive;

        [DataMember(Order = 10)]
        public Color InformationTextColor { get; set; } = Color.DarkBlue;

        [DataMember(Order = 11)]
        public Color CustomTextColor1 { get; set; } = Color.Purple;

        [DataMember(Order = 12)]
        public Color CustomTextColor2 { get; set; } = Color.DarkSalmon;

        [DataMember(Order = 13)]
        public Color CustomTextColor3 { get; set; } = Color.DarkOrange;

        [DataMember(Order = 14)]
        public Color CustomTextColor4 { get; set; } = Color.Brown;

        [DataMember(Order = 15)]
        public Color FindSearchTermColor { get; set; } = Color.Green;

        [DataMember(Order = 16)]
        public Color FindFileNameColor { get; set; } = Color.Gray;

        private static readonly string ProgramDataFolder;
        private static readonly string SettingsFile;

        public static event EventHandler SettingsChanged;

        private static void OnSettingsChanged(object sender, EventArgs ea) => SettingsChanged?.Invoke(sender, ea);

        static Settings()
        {
            ProgramDataFolder = Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "VSColorOutput");
            SettingsFile = Path.Combine(ProgramDataFolder, "vscoloroutput.json");
        }

        public static Settings Load()
        {
            Directory.CreateDirectory(ProgramDataFolder);
            if (!File.Exists(SettingsFile)) new Settings().Save();
            using (var stream = new FileStream(SettingsFile, FileMode.Open))
            {
                var deserialize = new DataContractJsonSerializer(typeof(Settings));
                return (Settings)deserialize.ReadObject(stream);
            }
        }

        public void Save()
        {
            Directory.CreateDirectory(ProgramDataFolder);
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