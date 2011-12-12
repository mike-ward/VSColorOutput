// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace BlueOnionSoftware
{
    [Guid("BE905985-26BB-492B-9453-743E26F4E8BB")]
    public class VsColorOutputOptions : DialogPage
    {
        public const string Category = "VSColorOutput";
        public const string SubCategory = "General";
        private const string RegExPatternsKey = "RegExPatterns";
        private const string RegistryPath = @"DialogPage\BlueOnionSoftware.VsColorOutputOptions";

        [DisplayName("RegEx Patterns")]
        public RegExClassification[] RegExPatterns { get; set; }

        public override void LoadSettingsFromStorage()
        {
            RegExPatterns = LoadPatterns();
        }

        public override void SaveSettingsToStorage()
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(RegExClassification[]));
                serializer.WriteObject(ms, RegExPatterns);
                var json = Encoding.Default.GetString(ms.ToArray());
                using (var root = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, false))
                using (var settings = root.OpenSubKey(RegistryPath, true))
                {
                    settings.SetValue(RegExPatternsKey, json, RegistryValueKind.String);
                    settings.Close();
                }
            }
        }

        public static bool UseDefaults { get; set; }

        public static RegExClassification[] LoadPatterns()
        {
            if (UseDefaults)
            {
                return DefaultPatterns();
            }
            using (var root = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, false))
            using (var settings = root.OpenSubKey(RegistryPath))
            {
                var json = settings.GetValue(RegExPatternsKey) as string;
                return string.IsNullOrEmpty(json) ? DefaultPatterns() : LoadPatternsFromJson(json);
            }
        }

        private static RegExClassification[] DefaultPatterns()
        {
            return new[]
            {
                new RegExClassification {RegExPattern = @"\+\+\+\>", ClassificationType = ClassificationTypes.LogSpecial, IgnoreCase = false},
                new RegExClassification {RegExPattern = @"(=====|-----)", ClassificationType = ClassificationTypes.BuildHead, IgnoreCase = false},
                new RegExClassification {RegExPattern = @"0 failed", ClassificationType = ClassificationTypes.BuildHead, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(\W|^)information\W", ClassificationType = ClassificationTypes.LogInformation, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(\W|^)warning\W", ClassificationType = ClassificationTypes.LogWarning, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(\W|^)(error|fail|failed|exception)\W", ClassificationType = ClassificationTypes.LogError, IgnoreCase = true}
            };
        }

        private static RegExClassification[] LoadPatternsFromJson(string json)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(RegExClassification[]));
                return (RegExClassification[])serializer.ReadObject(ms);
            }
        }

        public enum ClassificationTypes
        {
            BuildText,
            BuildHead,
            LogError,
            LogWarning,
            LogInformation,
            LogSpecial
        }

        public class RegExClassification
        {
            public string RegExPattern { get; set; }
            public ClassificationTypes ClassificationType { get; set; }
            public bool IgnoreCase { get; set; }

            public override string ToString()
            {
                return string.Format("\"{0}\",{1},{2}", RegExPattern, ClassificationType, IgnoreCase);
            }
        }
    }
}