// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System;
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

        [DisplayName("RegEx Patterns")]
        public RegExClassification[] RegExPatterns { get; set; }

        public override void LoadSettingsFromStorage()
        {
            using (var root = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, false))
            using (var settings = root.OpenSubKey(SettingsRegistryPath))
            {
                var json = settings.GetValue(RegExPatternsKey) as string;
                RegExPatterns = string.IsNullOrEmpty(json) ? DefaultPatterns() : LoadPatternsFromRegistry(json);
            }
        }

        public override void SaveSettingsToStorage()
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(RegExClassification[]));
                serializer.WriteObject(ms, RegExPatterns);
                var json = Encoding.Default.GetString(ms.ToArray());
                using (var root = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, false))
                using (var settings = root.OpenSubKey(SettingsRegistryPath, true))
                {
                    settings.SetValue(RegExPatternsKey, json, RegistryValueKind.String);
                    settings.Close();
                }
            }
        }

        private static RegExClassification[] DefaultPatterns()
        {
            return new[]
            {
                new RegExClassification {RegExPattern = "=====", ClassificationType = ClassificationTypes.BuildHead, IgnoreCase = true}
            };
        }

        private static RegExClassification[] LoadPatternsFromRegistry(string json)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(RegExClassification[]));
                return (RegExClassification[])serializer.ReadObject(ms);
            }
        }

        [Serializable]
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