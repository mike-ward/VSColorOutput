// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace BlueOnionSoftware
{
    public static class Settings
    {
        private const string RegExPatternsKey = "RegExPatterns";
        private const string RegistryPath = @"DialogPage\BlueOnionSoftware.VsColorOutputOptions";
        public static bool UseDefaultPatterns { get; set; } // Used for testing

        public static RegExClassification[] LoadPatterns()
        {
            if (UseDefaultPatterns)
            {
                return DefaultPatterns();
            }
            using (var root = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, false))
            {
                using (var settings = root.OpenSubKey(RegistryPath))
                {
                    var json = (settings != null) ? settings.GetValue(RegExPatternsKey) as string : null;
                    return (string.IsNullOrEmpty(json) || json == "[]") ? DefaultPatterns() : LoadPatternsFromJson(json);
                }
            }
        }

        private static RegExClassification[] DefaultPatterns()
        {
            return new[]
            {
                new RegExClassification {RegExPattern = @"\+\+\+\>", ClassificationType = ClassificationTypes.LogCustom1, IgnoreCase = false},
                new RegExClassification {RegExPattern = @"(=====|-----)", ClassificationType = ClassificationTypes.BuildHead, IgnoreCase = false},
                new RegExClassification {RegExPattern = @"0 failed", ClassificationType = ClassificationTypes.BuildHead, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(\W|^)(error|fail|failed|exception)\W", ClassificationType = ClassificationTypes.LogError, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(exception:|stack trace:)", ClassificationType = ClassificationTypes.LogError, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"^\s+at\s", ClassificationType = ClassificationTypes.LogError, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(\W|^)warning\W", ClassificationType = ClassificationTypes.LogWarning, IgnoreCase = true},
                new RegExClassification {RegExPattern = @"(\W|^)information\W", ClassificationType = ClassificationTypes.LogInformation, IgnoreCase = true}
            };
        }

        private static RegExClassification[] LoadPatternsFromJson(string json)
        {
            try
            {
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(RegExClassification[]));
                    var patterns = serializer.ReadObject(ms) as RegExClassification[];
                    return patterns ?? DefaultPatterns();
                }
            }
            catch (Exception)
            {
                return DefaultPatterns();
            }
        }

        public static void SaveSettingsToStorage(RegExClassification[] regExPatterns)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(RegExClassification[]));
                serializer.WriteObject(ms, regExPatterns);
                var json = Encoding.Default.GetString(ms.ToArray());
                using (var root = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, true))
                {
                    using (var settings = root.OpenSubKey(RegistryPath, true) ?? root.CreateSubKey(RegistryPath))
                    {
                        settings.SetValue(RegExPatternsKey, json, RegistryValueKind.String);
                    }
                }
                if (OutputClassifierProvider.OutputClassifier != null)
                {
                    OutputClassifierProvider.OutputClassifier.ReloadClassifiers();
                }
            }
        }
    }
}