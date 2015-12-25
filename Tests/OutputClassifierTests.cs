// Copyright (c) 2011 Blue Onion Software, All rights reserved

using System;
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
            var outputClassifier = new OutputClassifier(null, null);
            outputClassifier.GetClassificationSpans(new SnapshotSpan()).Should().BeEmpty();
        }

        [Test]
        public void GetClassificationSpansZeroLengthSnapShot()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockClassificationTypeRegistryService = new Mock<IClassificationTypeRegistryService>();
            var outputClassifier = new OutputClassifier(mockClassificationTypeRegistryService.Object, null);
            var mockSnapshot = new Mock<ITextSnapshot>();
            mockSnapshot.SetupGet(s => s.Length).Returns(0);
            var snapshotSpan = new SnapshotSpan(mockSnapshot.Object, 0, 0);
            outputClassifier.GetClassificationSpans(snapshotSpan).Should().BeEmpty();
            mockSnapshot.VerifyAll();
            mockServiceProvider.VerifyAll();
            mockClassificationTypeRegistryService.VerifyAll();
        }

        [TestCase("-----", OutputClassificationDefinitions.BuildHead)]
        [TestCase("=====", OutputClassificationDefinitions.BuildHead)]
        [TestCase("0 failed,", OutputClassificationDefinitions.BuildHead)]
        [TestCase("plain text", OutputClassificationDefinitions.BuildText)]
        [TestCase("+++>", OutputClassificationDefinitions.LogCustom1)]
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
            Settings.Load();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockClassificationTypeRegistryService = new Mock<IClassificationTypeRegistryService>();
            mockClassificationTypeRegistryService
                .Setup(c => c.GetClassificationType(classification))
                .Returns(new Mock<IClassificationType>().Object);

            var outputClassifier = new OutputClassifier(mockClassificationTypeRegistryService.Object, null);
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
            var spans = outputClassifier.GetClassificationSpans(snapshotSpan);
            spans.Should().NotBeEmpty();
            mockSnapshot.VerifyAll();
            mockSnapshotLine.VerifyAll();
            mockServiceProvider.VerifyAll();
            mockClassificationTypeRegistryService.VerifyAll();
        }
    }
}