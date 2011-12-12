// Copyright (c) 2011 Blue Onion Software, All rights reserved
using BlueOnionSoftware;
using FluentAssertions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class OutputClassifierTests
    {
        [Test]
        public void GetClassificationSpansNullSnapShot()
        {
            var outputClassifier = new OutputClassifier(null);
            outputClassifier.GetClassificationSpans(new SnapshotSpan()).Should().BeEmpty();
        }

        [Test]
        public void GetClassificationSpansZeroLengthSnapShot()
        {
            var classificationTypeRegistryService = new Mock<IClassificationTypeRegistryService>();
            var outputClassifier = new OutputClassifier(classificationTypeRegistryService.Object);
            var mockSnapshot = new Mock<ITextSnapshot>();
            mockSnapshot.SetupGet(s => s.Length).Returns(0);
            var snapshotSpan = new SnapshotSpan(mockSnapshot.Object, 0, 0);
            outputClassifier.GetClassificationSpans(snapshotSpan).Should().BeEmpty();
            mockSnapshot.VerifyAll();
        }

        [TestCase("-----", OutputClassificationDefinitions.BuildHead)]
        [TestCase("=====", OutputClassificationDefinitions.BuildHead)]
        [TestCase("0 failed,", OutputClassificationDefinitions.BuildHead)]
        [TestCase("plain text", OutputClassificationDefinitions.BuildText)]
        [TestCase("+++>", OutputClassificationDefinitions.LogSpecial)]
        [TestCase(":Error:", OutputClassificationDefinitions.LogError)]
        [TestCase("Error ", OutputClassificationDefinitions.LogError)]
        [TestCase(" Failed ", OutputClassificationDefinitions.LogError)]
        [TestCase("Failed ", OutputClassificationDefinitions.LogError)]
        [TestCase(" Fail ", OutputClassificationDefinitions.LogError)]
        [TestCase("Exception ", OutputClassificationDefinitions.LogError)]
        [TestCase(":Warning:", OutputClassificationDefinitions.LogWarn)]
        [TestCase("Warning:", OutputClassificationDefinitions.LogWarn)]
        [TestCase(":Information:", OutputClassificationDefinitions.LogInfo)]
        [TestCase("Information:", OutputClassificationDefinitions.LogInfo)]
        public void GetClassificationSpansFromSnapShot(string pattern, string classification)
        {
            var mockClassificationTypeRegistryService = new Mock<IClassificationTypeRegistryService>();
            mockClassificationTypeRegistryService
                .Setup(c => c.GetClassificationType(classification))
                .Returns(new Mock<IClassificationType>().Object);
            
            var outputClassifier = new OutputClassifier(mockClassificationTypeRegistryService.Object);
            var mockSnapshot = new Mock<ITextSnapshot>();
            var mockSnapshotLine = new Mock<ITextSnapshotLine>();

            mockSnapshot.SetupGet(s => s.Length).Returns(1);
            mockSnapshot.Setup(s => s.GetLineFromPosition(0)).Returns(mockSnapshotLine.Object);
            mockSnapshot.Setup(s => s.GetLineFromLineNumber(0)).Returns(mockSnapshotLine.Object);
            mockSnapshot.Setup(s => s.GetText(It.IsAny<Span>())).Returns(pattern);

            mockSnapshotLine.SetupGet(l => l.Start).Returns(new SnapshotPoint(mockSnapshot.Object, 0));
            mockSnapshotLine.SetupGet(l => l.Length).Returns(1);
            mockSnapshotLine.SetupGet(l => l.LineNumber).Returns(0);
            mockSnapshotLine.SetupGet(l => l.Snapshot).Returns(mockSnapshot.Object);

            var snapshotSpan = new SnapshotSpan(mockSnapshot.Object, 0, 1);
            VsColorOutputOptions.UseDefaults = true;
            var spans = outputClassifier.GetClassificationSpans(snapshotSpan);
            spans.Should().NotBeEmpty();
            mockClassificationTypeRegistryService.VerifyAll();
            mockSnapshot.VerifyAll();
            mockSnapshotLine.VerifyAll();
        }
    }
}