using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

// Disable warning for unused ClassificationChanged event
#pragma warning disable 67

namespace BlueOnionSoftware
{
    public class FindResultsClassifier : IClassifier
    {
        private const string FindAll = "Find all \"";
        private const string MatchCase = "Match case";
        private const string WholeWord = "Whole word";
        private const string ListFilenamesOnly = "List filenames only";

        private bool _settingsLoaded;
        private bool _highlightFindResults;
        private readonly IClassificationTypeRegistryService _classificationRegistry;
        private static readonly Regex FilenameRegex;

        private Regex _searchTextRegex;

        static FindResultsClassifier()
        {
            FilenameRegex = new Regex(@"^\s*.:.*\(\d+\):", RegexOptions.Compiled);
        }

        public FindResultsClassifier(IClassificationTypeRegistryService classificationRegistry)
        {
            _classificationRegistry = classificationRegistry;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            LoadSettings();
            var classifications = new List<ClassificationSpan>();

            var snapshot = span.Snapshot;
            if (snapshot == null || snapshot.Length == 0 || !CanSearch(span) || !_highlightFindResults)
            {
                return classifications;
            }

            var text = span.GetText();

            var filenameSpans = GetMatches(text, FilenameRegex, span.Start, FilenameClassificationType).ToList();
            var searchTermSpans = GetMatches(text, _searchTextRegex, span.Start, SearchTermClassificationType).ToList();

            var toRemove = (from searchSpan in searchTermSpans
                from filenameSpan in filenameSpans
                where filenameSpan.Span.Contains(searchSpan.Span)
                select searchSpan).ToList();

            classifications.AddRange(filenameSpans);
            classifications.AddRange(searchTermSpans.Except(toRemove));
            return classifications;
        }

        private bool CanSearch(SnapshotSpan span)
        {
            if (span.Start.Position != 0 && _searchTextRegex != null)
            {
                return true;
            }
            _searchTextRegex = null;
            var firstLine = span.Snapshot.GetLineFromLineNumber(0).GetText();
            if (firstLine.StartsWith(FindAll))
            {
                var strings = (from s in firstLine.Split(',')
                    select s.Trim()).ToList();

                var start = strings[0].IndexOf('"');
                var searchTerm = strings[0].Substring(start + 1, strings[0].Length - start - 2);
                var matchCase = strings.Contains(MatchCase);
                var matchWholeWord = strings.Contains(WholeWord);
                var filenamesOnly = strings.Contains(ListFilenamesOnly);

                if (!filenamesOnly)
                {
                    var regex = matchWholeWord ? $@"\b{Regex.Escape(searchTerm)}\b" : Regex.Escape(searchTerm);
                    var casing = matchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                    _searchTextRegex = new Regex(regex, RegexOptions.None | casing);

                    return true;
                }
            }
            return false;
        }

        private void LoadSettings()
        {
            if (_settingsLoaded) return;
            var settings = Settings.Load();
            _highlightFindResults = settings.HighlightFindResults;
            _settingsLoaded = true;
        }

        public void ClearSettings()
        {
            _settingsLoaded = false;
        }

        private static IEnumerable<ClassificationSpan> GetMatches(string text, Regex regex, SnapshotPoint snapStart, IClassificationType classificationType)
        {
            return from match in regex.Matches(text).Cast<Match>()
                select new ClassificationSpan(new SnapshotSpan(snapStart + match.Index, match.Length), classificationType);
        }

        private IClassificationType SearchTermClassificationType => _classificationRegistry.GetClassificationType(OutputClassificationDefinitions.FindResultsSearchTerm);

        private IClassificationType FilenameClassificationType => _classificationRegistry.GetClassificationType(OutputClassificationDefinitions.FindResultsFilename);

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}