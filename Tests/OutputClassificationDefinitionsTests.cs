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
            format.ForegroundColor.Should().Be(Colors.Olive);
        }

        [Test]
        public void LogInfoFormat()
        {
            var format = new OutputClassificationDefinitions.LogInformationFormat();
            format.DisplayName.Should().Be("VSColorOutput Log Information");
            format.ForegroundColor.Should().Be(Colors.DarkBlue);
        }

        [Test]
        public void LogCustom1Format()
        {
            var format = new OutputClassificationDefinitions.LogCustom1Format();
            format.DisplayName.Should().Be("VSColorOutput Log Custom1");
            format.ForegroundColor.Should().Be(Colors.Purple);
        }

        [Test]
        public void LogCustom2Format()
        {
            var format = new OutputClassificationDefinitions.LogCustom2Format();
            format.DisplayName.Should().Be("VSColorOutput Log Custom2");
            format.ForegroundColor.Should().Be(Colors.DarkSalmon);
        }

        [Test]
        public void LogCustom3Format()
        {
            var format = new OutputClassificationDefinitions.LogCustom3Format();
            format.DisplayName.Should().Be("VSColorOutput Log Custom3");
            format.ForegroundColor.Should().Be(Colors.DarkOrange);
        }

        [Test]
        public void LogCustom4Format()
        {
            var format = new OutputClassificationDefinitions.LogCustom4Format();
            format.DisplayName.Should().Be("VSColorOutput Log Custom4");
            format.ForegroundColor.Should().Be(Colors.Brown);
        }

        [Test]
        public void FindResultsSearchTermFormat()
        {
            var format = new OutputClassificationDefinitions.FindResultsSearchTermFormat();
            format.DisplayName.Should().Be("VSColorOutput Find Results Search Term");
            format.ForegroundColor.Should().Be(Colors.Green);
        }

        [Test]
        public void FindResultsFilenameFormat()
        {
            var format = new OutputClassificationDefinitions.FindResultsFilenameFormat();
            format.DisplayName.Should().Be("VSColorOutput Find Results Filename");
            format.ForegroundColor.Should().Be(Colors.Gray);
        }
    }
}