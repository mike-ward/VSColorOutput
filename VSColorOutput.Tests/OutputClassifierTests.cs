using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Moq;
using VSColorOutput.Output.ColorClassifier;
using VSColorOutput.State;

namespace Tests
{
    [TestClass]
    public class OutputClassifierTests
    {
        [TestMethod]
        public void GetClassificationSpansNullSnapShot()
        {
            var outputClassifier = new OutputClassifier();
            outputClassifier.Initialize(null, null);
            outputClassifier.GetClassificationSpans(new SnapshotSpan()).Should().BeEmpty();
        }

        [TestMethod]
        public void GetClassificationSpansZeroLengthSnapShot()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockClassificationTypeRegistryService = new Mock<IClassificationTypeRegistryService>();
            var outputClassifier = new OutputClassifier();
            outputClassifier.Initialize(mockClassificationTypeRegistryService.Object, null);
            var mockSnapshot = new Mock<ITextSnapshot>();
            mockSnapshot.SetupGet(s => s.Length).Returns(0);
            var snapshotSpan = new SnapshotSpan(mockSnapshot.Object, 0, 0);
            outputClassifier.GetClassificationSpans(snapshotSpan).Should().BeEmpty();

            Mock.VerifyAll();
        }

        [DataRow("-----", ClassificationTypeDefinitions.BuildHead)]
        [DataRow("=====", ClassificationTypeDefinitions.BuildHead)]
        [DataRow("0 failed,", ClassificationTypeDefinitions.BuildHead)]
        [DataRow("plain text", ClassificationTypeDefinitions.BuildText)]
        [DataRow("+++>", ClassificationTypeDefinitions.LogCustom1)]
        [DataRow(":Error:", ClassificationTypeDefinitions.LogError)]
        [DataRow("Error ", ClassificationTypeDefinitions.LogError)]
        [DataRow("/errorReport:prompt ", ClassificationTypeDefinitions.BuildText)]
        [DataRow(" Failed ", ClassificationTypeDefinitions.LogError)]
        [DataRow("Failed ", ClassificationTypeDefinitions.LogError)]
        [DataRow(" Fail ", ClassificationTypeDefinitions.LogError)]
        [DataRow("Exception ", ClassificationTypeDefinitions.LogError)]
        [DataRow(":Warning:", ClassificationTypeDefinitions.LogWarn)]
        [DataRow("Warning:", ClassificationTypeDefinitions.LogWarn)]
        [DataRow(":Information:", ClassificationTypeDefinitions.LogInfo)]
        [DataRow("Information:", ClassificationTypeDefinitions.LogInfo)]
        [DataRow(" 0 Warning(s)", ClassificationTypeDefinitions.BuildHead)]
        [DataRow(" 0 Error(s)", ClassificationTypeDefinitions.BuildHead)]
        [DataRow("Could not find file", ClassificationTypeDefinitions.LogError)]
        [DataRow("warning CS0168: The variable \'exception\'", ClassificationTypeDefinitions.LogWarn)]
        [DataRow("[11:15:27.531394] info: some mundane information message", ClassificationTypeDefinitions.LogInfo)]
        [DataRow("[11:15:27.531394] warn: a message to notify a user", ClassificationTypeDefinitions.LogWarn)]
        [DataRow("[11:15:27.531394] trce: random trace message", ClassificationTypeDefinitions.BuildText)]
        [DataRow("[11:15:27.531394] dbug: random debug message", ClassificationTypeDefinitions.BuildText)]
        [DataRow("[11:15:27.531394] fail: failure description", ClassificationTypeDefinitions.LogError)]
        [DataRow("[11:15:27.531394] crit: failure description", ClassificationTypeDefinitions.LogError)]
        [DataTestMethod]
        public void GetClassificationSpansFromSnapShot(string pattern, string classification)
        {
            Settings.Load();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockClassificationTypeRegistryService = new Mock<IClassificationTypeRegistryService>();
            mockClassificationTypeRegistryService
               .Setup(c => c.GetClassificationType(classification))
               .Returns(new Mock<IClassificationType>().Object);

            var outputClassifier = new OutputClassifier();
            outputClassifier.Initialize(mockClassificationTypeRegistryService.Object, null);
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

            Mock.VerifyAll();
        }
    }
}