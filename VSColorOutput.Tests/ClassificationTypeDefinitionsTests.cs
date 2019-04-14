using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSColorOutput.State;
using static VSColorOutput.Output.ColorClassifier.ClassificationTypeDefinitions;

namespace Tests
{
    [TestClass]
    public class ClassificationTypeDefinitionsTests
    {
        private Settings _settings;

        [TestInitialize]
        public void Setup()
        {
            _settings = new Settings();
        }

        [TestMethod]
        public void BuildHeaderFormat()
        {
            var format = new BuildHeaderFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.BuildMessageColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void BuildTextFormat()
        {
            var format = new BuildTextFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.BuildTextColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void LogErrorFormat()
        {
            var format = new LogErrorFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.ErrorTextColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void LogWarnFormat()
        {
            var format = new LogWarningFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.WarningTextColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void LogInfoFormat()
        {
            var format = new LogInformationFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.InformationTextColor));
        }

        [TestMethod]
        public void LogCustom1Format()
        {
            var format = new LogCustom1Format();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.CustomTextColor1));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void LogCustom2Format()
        {
            var format = new LogCustom2Format();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.CustomTextColor2));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void LogCustom3Format()
        {
            var format = new LogCustom3Format();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.CustomTextColor3));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void LogCustom4Format()
        {
            var format = new LogCustom4Format();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.CustomTextColor4));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void FindResultsSearchTermFormat()
        {
            var format = new FindResultsSearchTermFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.FindSearchTermColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void FindResultsFilenameFormat()
        {
            var format = new FindResultsFilenameFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.FindFileNameColor));
            format.BackgroundOpacity.Should().Be(0);
        }

        [TestMethod]
        public void TimeStampFormat()
        {
            var format = new TimeStampFormat();
            format.ForegroundColor.Should().Be(ToMediaColor(_settings.TimeStampColor));
            format.BackgroundOpacity.Should().Be(0);
        }
    }
}