// Copyright (c) 2011 Siemens Medical Solutions, USA - All rights reserved
using System.Windows.Media;
using BlueOnionSoftware;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class OutputClassificationDefinitionsTests
    {
        [Test]
        public void BuildHeaderFormat()
        {
            var format = new OutputClassificationDefinitions.BuildHeaderFormat();
            format.DisplayName.Should().Be("VSColorOutput Build Header");
            format.ForegroundColor.Should().Be(Colors.Green);
        }

        [Test]
        public void BuildTextFormat()
        {
            var format = new OutputClassificationDefinitions.BuildTextFormat();
            format.DisplayName.Should().Be("VSColorOutput Build Text");
            format.ForegroundColor.Should().Be(Colors.Gray);
        }

        [Test]
        public void LogSpecialFormat()
        {
            var format = new OutputClassificationDefinitions.LogSpecialFormat();
            format.DisplayName.Should().Be("VSColorOutput Log Special");
            format.ForegroundColor.Should().Be(Colors.Purple);
        }

        [Test]
        public void LogErrorFormat()
        {
            var format = new OutputClassificationDefinitions.LogErrorFormat();
            format.DisplayName.Should().Be("VSColorOutput Log Error");
            format.ForegroundColor.Should().Be(Colors.Red);
        }

        [Test]
        public void LogWarnFormat()
        {
            var format = new OutputClassificationDefinitions.LogWarningFormat();
            format.DisplayName.Should().Be("VSColorOutput Log Warning");
            format.ForegroundColor.Should().Be(Colors.DarkGoldenrod);
        }

        [Test]
        public void LogInfoFormat()
        {
            var format = new OutputClassificationDefinitions.LogInformationFormat();
            format.DisplayName.Should().Be("VSColorOutput Log Information");
            format.ForegroundColor.Should().Be(Colors.DarkBlue);
        }
    }
}