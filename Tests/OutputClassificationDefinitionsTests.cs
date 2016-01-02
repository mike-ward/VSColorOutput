using System.Windows.Media;
using FluentAssertions;
using NUnit.Framework;
using VSColorOutput.Output.ColorClassifier;

namespace Tests
{
    [TestFixture]
    public class OutputClassificationDefinitionsTests
    {
        [Test]
        public void BuildHeaderFormat()
        {
            var format = new OutputClassificationDefinitions.BuildHeaderFormat();
            format.ForegroundColor.Should().Be(Colors.Green);
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void BuildTextFormat()
        {
            var format = new OutputClassificationDefinitions.BuildTextFormat();
            format.ForegroundColor.Should().Be(Colors.Gray);
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogErrorFormat()
        {
            var format = new OutputClassificationDefinitions.LogErrorFormat();
            format.ForegroundColor.Should().Be(Colors.Red);
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogWarnFormat()
        {
            var format = new OutputClassificationDefinitions.LogWarningFormat();
            format.ForegroundColor.Should().Be(Colors.Olive);
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogInfoFormat()
        {
            var format = new OutputClassificationDefinitions.LogInformationFormat();
            format.ForegroundColor.Should().Be(Colors.DarkBlue);
        }

        [Test]
        public void LogCustom1Format()
        {
            var format = new OutputClassificationDefinitions.LogCustom1Format();
            format.ForegroundColor.Should().Be(Colors.Purple);
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogCustom2Format()
        {
            var format = new OutputClassificationDefinitions.LogCustom2Format();
            format.ForegroundColor.Should().Be(Colors.DarkSalmon);
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogCustom3Format()
        {
            var format = new OutputClassificationDefinitions.LogCustom3Format();
            format.ForegroundColor.Should().Be(Colors.DarkOrange);
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void LogCustom4Format()
        {
            var format = new OutputClassificationDefinitions.LogCustom4Format();
            format.ForegroundColor.Should().Be(Colors.Brown);
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void FindResultsSearchTermFormat()
        {
            var format = new OutputClassificationDefinitions.FindResultsSearchTermFormat();
            format.ForegroundColor.Should().Be(Colors.Green);
            format.BackgroundOpacity.Should().Be(0);
        }

        [Test]
        public void FindResultsFilenameFormat()
        {
            var format = new OutputClassificationDefinitions.FindResultsFilenameFormat();
            format.ForegroundColor.Should().Be(Colors.Gray);
            format.BackgroundOpacity.Should().Be(0);
        }
    }
}