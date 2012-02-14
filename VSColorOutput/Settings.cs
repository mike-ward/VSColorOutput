// Copyright (c) 2012 Blue Onion Software, All rights reserved
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BlueOnionSoftware
{
    public class Settings
    {
        public const string RegExPatternsKey = "RegExPatterns";
        public const string StopOnBuildErrorKey = "StopOnBuildError";
        public const string ShowElapsedBuildTimeKey = "ShowElapsedBuildTime";
        public const string ShowDebugWindowOnDebugKey = "ShowDebugWindowOnDebug";
        public const string RegistryPath = @"DialogPage\BlueOnionSoftware.VsColorOutputOptions";
        public static IRegistryKey OverrideRegistryKey { get; set; }

        public RegExClassification[] Patterns { get; set; }
        public bool EnableStopOnBuildError { get; set; }
        public bool ShowElapsedBuildTime { get; set; }
        public bool ShowDebugWindowOnDebug { get; set; }

        public void Load()
        {
            using (var key = OpenRegistry(false))
            {
                var json = (key != null) ? key.GetValue(RegExPatternsKey) as string : null;
                Patterns = (string.IsNullOrEmpty(json) || json == "[]") ? DefaultPatterns() : LoadPatternsFromJson(json);
                
                var stopOnBuildError = (key != null) ? key.GetValue(StopOnBuildErrorKey) as string : bool.FalseString;
                EnableStopOnBuildError = string.IsNullOrEmpty(stopOnBuildError) == false && stopOnBuildError == bool.TrueString;

                var showElapsedBuildTime = (key != null) ? key.GetValue(ShowElapsedBuildTimeKey) as string : bool.FalseString;
                ShowElapsedBuildTime = string.IsNullOrEmpty(showElapsedBuildTime) == false && showElapsedBuildTime == bool.TrueString;
                
                var showDebugWindowOnDebug = (key != null) ? key.GetValue(ShowDebugWindowOnDebugKey) as string : bool.FalseString;
                ShowDebugWindowOnDebug = string.IsNullOrEmpty(showDebugWindowOnDebug) == false && showDebugWindowOnDebug == bool.TrueString;
            }
        }

        public void Save()
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(RegExClassification[]));
                serializer.WriteObject(ms, Patterns);
                var json = Encoding.Default.GetString(ms.ToArray());
                using (var key = OpenRegistry(true))
                {
                    key.SetValue(RegExPatternsKey, json);
                    key.SetValue(StopOnBuildErrorKey, EnableStopOnBuildError.ToString(CultureInfo.InvariantCulture));
                    key.SetValue(ShowElapsedBuildTimeKey, ShowElapsedBuildTime.ToString(CultureInfo.InvariantCulture));
                    key.SetValue(ShowDebugWindowOnDebugKey, ShowDebugWindowOnDebug.ToString(CultureInfo.InvariantCulture));
                }
                if (OutputClassifierProvider.OutputClassifier != null)
                {
                    OutputClassifierProvider.OutputClassifier.ClearSettings();
                }
            }
        }

        private static IRegistryKey OpenRegistry(bool writeable)
        {
            if (OverrideRegistryKey != null)
            {
                return OverrideRegistryKey;
            }
            var root = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, writeable);
            var subKey = writeable 
                ? root.CreateSubKey(RegistryPath)
                : root.OpenSubKey(RegistryPath);
            return (subKey != null) ? new RegistryKeyImpl(subKey) : null;
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
    }
}