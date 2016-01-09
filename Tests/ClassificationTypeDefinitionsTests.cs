using FluentAssertions;
using NUnit.Framework;
using VSColorOutput.State;
using static VSColorOutput.Output.ColorClassifier.ClassificationTypeDefinitions;

namespace Tests
{
    [TestFixture]
    public class ClassificationTypeDefinitionsTests
    {
        private Settings _settings;

        [SetUp]
        public void Setup()
        {
            _settings = new Settings();
        }

        [Test]
        public void BuildHeaderFormat()
        {
            var format = new BuildHeaderFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.BuildMessageColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void BuildTextFormat()
        {
            var format = new BuildTextFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.BuildTextColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogErrorFormat()
        {
            var format = new LogErrorFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.ErrorTextColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogWarnFormat()
        {
            var format = new LogWarningFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.WarningTextColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogInfoFormat()
        {
            var format = new LogInformationFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.InformationTextColor));
        }

        [Test]
        public void LogCustom1Format()
        {
            var format = new LogCustom1Format();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.CustomTextColor1));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogCustom2Format()
        {
            var format = new LogCustom2Format();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.CustomTextColor2));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogCustom3Format()
        {
            var format = new LogCustom3Format();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.CustomTextColor3));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogCustom4Format()
        {
            var format = new LogCustom4Format();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.CustomTextColor4));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void FindResultsSearchTermFormat()
        {
            var format = new FindResultsSearchTermFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.FindSearchTermColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void FindResultsFilenameFormat()
        {
            var format = new FindResultsFilenameFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.FindFileNameColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void TimestampFormat()
        {
            var format = new TimestampFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.TimestampColor));
            format.BackgroundOpacity.Should().Be(0);
        }
    }
}