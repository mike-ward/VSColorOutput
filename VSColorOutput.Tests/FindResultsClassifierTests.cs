using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Moq;
using VSColorOutput.FindResults;
using VSColorOutput.Output.ColorClassifier;

namespace Tests
{
    [TestClass]
    public class FindResultsClassifierTests
    {
        private FindResultsClassifier _classifier;

        private const string ResultsPreamble = "Find all \"";
        private const string ResultsPreambleEnd = "\"";
        private const string ResultsMatchCase = "Match case";
        private const string ResultsMatchWord = "Whole word";
        private const string ResultsUsingRegex = "Regular expressions";
        private const string ResultsUsingWildcards = "Wildcards";
        private const string ResultsSubfolders = "SubFolders";
        private const string ResultsFilenamesOnly = "List filenames only";
        private const string ResultsLookIn = "\"Entire Solution\"";
        private const string ResultsFileTypes = "\"*.cs\"";

        private const string UsingResultsLine1 = @"  C:\Projects\App\Program.cs(1):using System;";
        private const string UsingResultsLine2 = @"  C:\Projects\App\Program.cs(20):using System.Collections.Generic;";

        private const string UsingUncResultsLine1 = @"  \\Projects\App\Program.cs(1):using System;";
        private const string UsingUncResultsLine2 = @"  \\Projects\App\Program.cs(20):using System.Collections.Generic;";

        private const string UsingResultsLineWithMultipleHits = @"  C:\Projects\App\Program.cs(2):using System.Collections.Generic; // using UnusedStuff;";

        private const string MixedCaseResults = @"  C:\Projects\App\Program.cs(366): // casing Casing CASING casing";

        private const string CaseInsensitiveWholeWordResults = @"  C:\Projects\App\Program.cs(200):public class Classifier // subclass";

        [TestInitialize]
        public void Setup()
        {
            var mockSearchTermClassification = new Mock<IClassificationType>();
            mockSearchTermClassification.Setup(c => c.IsOfType(ClassificationTypeDefinitions.FindResultsSearchTerm)).Returns(true);

            var mockClassificationTypeRegistryService = new Mock<IClassificationTypeRegistryService>();
            mockClassificationTypeRegistryService
               .Setup(c => c.GetClassificationType(It.IsAny<string>()))
               .Returns((string classificationType) => new FakeClassificationType(classificationType));

            _classifier = new FindResultsClassifier();
            _classifier.Initialize(mockClassificationTypeRegistryService.Object, null);
            _classifier.GetClassificationSpans(new SnapshotSpan());
            _classifier.HighlightFindResults = true;
        }

        [TestMethod]
        public void GetClassificationSpansNullSnapShot()
        {
            _classifier.GetClassificationSpans(new SnapshotSpan()).Should().BeEmpty();
        }

        [TestMethod]
        public void GetClassificationSpansZeroLengthSnapShot()
        {
            var mockSnapshot = new Mock<ITextSnapshot>();
            mockSnapshot.SetupGet(s => s.Length).Returns(0);
            var snapshotSpan = new SnapshotSpan(mockSnapshot.Object, 0, 0);

            _classifier.GetClassificationSpans(snapshotSpan).Should().BeEmpty();
        }

        [TestMethod]
        public void ClassifiesSearchTermInSearchResultsBanner()
        {
            const string searchTerm = "using";
            var text = GetCaseInsensitiveResultsText(searchTerm, UsingResultsLine1, UsingResultsLine2);

            var offset1 = text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 0);

            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans = spans.Where(IsSearchTerm).ToList();

            spans.Count.Should().Be(1);

            AssertSearchTermClassified(spans[0], snapshotSpan, searchTerm, offset1);
        }

        [TestMethod]
        public void SearchTermContainingRegularExpressionCharactersDoesNotThrowException()
        {
            const string searchTerm = @"\P][^";
            var text = GetCaseInsensitiveResultsText(searchTerm, UsingResultsLine1, UsingResultsLine2);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);

            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans = spans.Where(IsSearchTerm).ToList();

            spans.Count.Should().Be(0);
        }

        [TestMethod]
        public void ClassifiesSearchTermInFirstLineOfResults()
        {
            const string searchTerm = "using";
            var text = GetCaseInsensitiveResultsText(searchTerm, UsingResultsLine1, UsingResultsLine2);

            var offset1 = UsingResultsLine1.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans = spans.Where(IsSearchTerm).ToList();

            spans.Count.Should().Be(1);

            AssertSearchTermClassified(spans[0], snapshotSpan, searchTerm, offset1);
        }

        [TestMethod]
        public void ClassifiesSearchTermInSubsequentLinesOfResults()
        {
            const string searchTerm = "using";
            var text = GetCaseInsensitiveResultsText(searchTerm, UsingResultsLine1, UsingResultsLine2);

            var offset1 = UsingResultsLine2.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 2);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans = spans.Where(IsSearchTerm).ToList();

            spans.Count.Should().Be(1);

            AssertSearchTermClassified(spans[0], snapshotSpan, searchTerm, offset1);
        }

        [TestMethod]
        public void ClassifiesSearchTermMultipleTimesInText()
        {
            const string searchTerm = "using";
            var text = GetCaseInsensitiveResultsText(searchTerm, UsingResultsLineWithMultipleHits);

            var offset1 = UsingResultsLineWithMultipleHits.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);
            var offset2 = UsingResultsLineWithMultipleHits.IndexOf(searchTerm, offset1 + searchTerm.Length, StringComparison.OrdinalIgnoreCase);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans = spans.Where(IsSearchTerm).ToList();

            spans.Count.Should().Be(2);

            AssertSearchTermClassified(spans[0], snapshotSpan, searchTerm, offset1);
            AssertSearchTermClassified(spans[1], snapshotSpan, searchTerm, offset2);
        }

        [TestMethod]
        public void ClassifiesAllSearchTermsCaseInsensitive()
        {
            const string searchTerm = "casing";
            var text = GetCaseInsensitiveResultsText(searchTerm, MixedCaseResults);

            var offset1 = MixedCaseResults.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);
            var offset2 = MixedCaseResults.IndexOf(searchTerm, offset1 + searchTerm.Length, StringComparison.OrdinalIgnoreCase);
            var offset3 = MixedCaseResults.IndexOf(searchTerm, offset2 + searchTerm.Length, StringComparison.OrdinalIgnoreCase);
            var offset4 = MixedCaseResults.IndexOf(searchTerm, offset3 + searchTerm.Length, StringComparison.OrdinalIgnoreCase);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans = spans.Where(IsSearchTerm).ToList();

            spans.Count.Should().Be(4);

            AssertSearchTermClassified(spans[0], snapshotSpan, searchTerm, offset1);
            AssertSearchTermClassified(spans[1], snapshotSpan, searchTerm, offset2);
            AssertSearchTermClassified(spans[2], snapshotSpan, searchTerm, offset3);
            AssertSearchTermClassified(spans[3], snapshotSpan, searchTerm, offset4);
        }

        [TestMethod]
        public void ClassifiesOnlySearchTermsMatchingCase()
        {
            const string searchTerm = "Casing";
            var text = GetCaseSensitiveResultsText(searchTerm, MixedCaseResults);

            var offset1 = MixedCaseResults.IndexOf(searchTerm, StringComparison.Ordinal);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans = spans.Where(IsSearchTerm).ToList();

            spans.Count.Should().Be(1);

            AssertSearchTermClassified(spans[0], snapshotSpan, searchTerm, offset1);
        }

        [TestMethod]
        public void ClassifiesWholeWordsOnly()
        {
            const string searchTerm = "class";
            var text = GetCaseInsensitiveWholeWordResultsText(searchTerm, CaseInsensitiveWholeWordResults);

            var offset1 = CaseInsensitiveWholeWordResults.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans = spans.Where(IsSearchTerm).ToList();

            spans.Count.Should().Be(1);

            AssertSearchTermClassified(spans[0], snapshotSpan, searchTerm, offset1);
        }

        [TestMethod]
        public void ClassifiesFilename()
        {
            const string searchTerm = "not found";
            var text = GetCaseInsensitiveResultsText(searchTerm, UsingResultsLine1, UsingResultsLine2);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans.Count.Should().Be(1);

            AssertFilenameClassified(spans[0], snapshotSpan);
        }

        [TestMethod]
        public void ClassifiesUncFilename()
        {
            const string searchTerm = "not found";
            var text = GetCaseInsensitiveResultsText(searchTerm, UsingUncResultsLine1, UsingUncResultsLine2);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans.Count.Should().Be(1);

            AssertFilenameClassified(spans[0], snapshotSpan);
        }

        [TestMethod]
        public void DoesNotClassifySearchTermInFilename()
        {
            const string searchTerm = @"C:\Projects";
            var text = GetCaseInsensitiveResultsText(searchTerm, UsingResultsLine1, UsingResultsLine2);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            var searchSpans = spans.Where(IsSearchTerm);
            searchSpans.Count().Should().Be(0);

            spans.Count.Should().Be(1);

            AssertFilenameClassified(spans[0], snapshotSpan);
        }

        [TestMethod]
        public void DoesNotClassifyAnythingWhenListingFilenamesOnly()
        {
            const string searchTerm = "using";
            var intro = BuildFindResultsBanner(searchTerm, filenamesOnly: true);
            var text = BuildResultsLines(intro, UsingResultsLine1, UsingResultsLine2);

            PrimeClassifierSearchOptionsWithFirstLine(text);

            var snapshotSpan = BuildSnapshotSpanFromLineNumber(text, 1);
            var spans = _classifier.GetClassificationSpans(snapshotSpan);

            spans.Count.Should().Be(0);
        }

        private static bool IsSearchTerm(ClassificationSpan s)
        {
            return s.ClassificationType.Classification == ClassificationTypeDefinitions.FindResultsSearchTerm;
        }

        private static void AssertFilenameClassified(ClassificationSpan classificationSpan, SnapshotSpan snapshotSpan)
        {
            var text = snapshotSpan.GetText();
            var index = Regex.Match(text, @"[:\\]").Index;
            index = text.IndexOf(':', index + 1);
            AssertClassification(classificationSpan, ClassificationTypeDefinitions.FindResultsFilename, snapshotSpan.Start.Position, index + 1);
        }

        private static void AssertSearchTermClassified(ClassificationSpan classificationSpan, SnapshotSpan snapshotSpan, string searchTerm, int searchTermStart)
        {
            AssertClassification(classificationSpan, ClassificationTypeDefinitions.FindResultsSearchTerm, snapshotSpan.Start.Position + searchTermStart, searchTerm.Length);
        }

        private static void AssertClassification(ClassificationSpan classificationSpan, string classificationType, int spanStart, int spanLength)
        {
            classificationSpan.ClassificationType.Classification.Should().Match(classificationType);
            classificationSpan.Span.Start.Position.Should().Be(spanStart);
            classificationSpan.Span.Length.Should().Be(spanLength);
        }

        private void PrimeClassifierSearchOptionsWithFirstLine(string text)
        {
            var introSnapshotSpan = BuildSnapshotSpanFromLineNumber(text, 0);
            _classifier.GetClassificationSpans(introSnapshotSpan);
        }

        private static string GetCaseInsensitiveResultsText(string searchTerm, params string[] resultLines)
        {
            var intro = BuildFindResultsBanner(searchTerm);
            return BuildResultsLines(intro, resultLines);
        }

        private static string GetCaseSensitiveResultsText(string searchTerm, params string[] resultLines)
        {
            var intro = BuildFindResultsBanner(searchTerm, true);
            return BuildResultsLines(intro, resultLines);
        }

        private static string GetCaseInsensitiveWholeWordResultsText(string searchTerm, params string[] resultLines)
        {
            var intro = BuildFindResultsBanner(searchTerm, matchWord: true);
            return BuildResultsLines(intro, resultLines);
        }

        private static string BuildFindResultsBanner(string searchTerm, bool caseSensitive = false, bool matchWord = false,
                                                     bool usingRegularExpressions = false, bool usingWildcards = false, bool filenamesOnly = false, bool filterByFiles = false)
        {
            var strings = new List<string> { ResultsPreamble + searchTerm + ResultsPreambleEnd };

            if (caseSensitive)
                strings.Add(ResultsMatchCase);
            if (matchWord)
                strings.Add(ResultsMatchWord);
            if (usingRegularExpressions)
                strings.Add(ResultsUsingRegex);
            if (usingWildcards)
                strings.Add(ResultsUsingWildcards);
            strings.Add(ResultsSubfolders);
            if (filenamesOnly)
                strings.Add(ResultsFilenamesOnly);
            strings.Add(ResultsLookIn);
            if (filterByFiles)
                strings.Add(ResultsFileTypes);

            return string.Join(", ", strings.ToArray());
        }

        private static string BuildResultsLines(string firstLine, params string[] lines)
        {
            return firstLine + Environment.NewLine + string.Join(Environment.NewLine, lines);
        }

        private static SnapshotSpan BuildSnapshotSpanFromLineNumber(string text, int lineNumber)
        {
            var textSnapshot = new FakeTextSnapshot(text);
            var textSnapshotLine = textSnapshot.GetLineFromLineNumber(lineNumber);
            return new SnapshotSpan(textSnapshot, textSnapshotLine.Start, textSnapshotLine.Length);
        }
    }
}