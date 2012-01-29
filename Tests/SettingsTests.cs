// Copyright (c) 2012 Blue Onion Software. All rights reserved.

using BlueOnionSoftware;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class SettingsTests
    {
        [Test]
        public void LoadShouldQueryRegistryForSettings()
        {
            var registry = new Mock<IRegistryKey>();
            Settings.OverrideRegistryKey = registry.Object;
            try
            {
                var settings = new Settings();
                settings.Load();
                registry.Verify(r => r.GetValue(Settings.RegExPatternsKey));
                registry.Verify(r => r.GetValue(Settings.StopOnBuildErrorKey));
                registry.Verify(r => r.GetValue(Settings.ShowElapsedBuildTimeKey));
                registry.Verify(r => r.GetValue(Settings.ShowDebugWindowOnDebugKey));
            }
            finally
            {
                Settings.OverrideRegistryKey = null;
            }
        }

        [Test]
        public void SaveShouldWriteSettingsToRegistry()
        {
            var registry = new Mock<IRegistryKey>();
            Settings.OverrideRegistryKey = registry.Object;
            try
            {
                var settings = new Settings();
                settings.Save();
                registry.Verify(r => r.SetValue(Settings.RegExPatternsKey, "null"));
                registry.Verify(r => r.SetValue(Settings.StopOnBuildErrorKey, bool.FalseString));
                registry.Verify(r => r.SetValue(Settings.ShowElapsedBuildTimeKey, bool.FalseString));
                registry.Verify(r => r.SetValue(Settings.ShowDebugWindowOnDebugKey, bool.FalseString));
            }
            finally
            {
                Settings.OverrideRegistryKey = null;
            }
        }
    }
}