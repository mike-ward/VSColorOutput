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
        private readonly IClassificationTypeRegistryService classificationRegistry;
        private static readonly Regex SearchOptionsRegex;
        private static readonly Regex FilenameRegex;

        private Regex searchTextRegex;

        static FindResultsClassifier()
        {
            // TODO: What about localised instances of VS?
            // We could also support searching by wildcard, it looks like it could be parsed into .net regex
            // But searching by regex? You're on your own. VS has implemented their own regex language. Sheesh.
            SearchOptionsRegex = new Regex("Find all \"(?<searchTerm>[^\"]+)\", (?<casing>Match case, )?(?<wholeWord>Whole word, )?", RegexOptions.Compiled);
            FilenameRegex = new Regex(@"^\s*.:.*\(\d+\):", RegexOptions.Compiled);
        }

        public FindResultsClassifier(IClassificationTypeRegistryService classificationRegistry)
        {
            this.classificationRegistry = classificationRegistry;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var classifications = new List<ClassificationSpan>();

            var snapshot = span.Snapshot;
            if (snapshot == null || snapshot.Length == 0 || !CanSearch(span))
                return classifications;

            var text = span.GetText();

            var filenameSpans = GetMatches(text, FilenameRegex, span.Start, FilenameClassificationType).ToList();
            var searchTermSpans = GetMatches(text, searchTextRegex, span.Start, SearchTermClassificationType).ToList();

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
            if (span.Start.Position != 0 && searchTextRegex != null)
                return true;

            searchTextRegex = null;

            var firstLine = span.Snapshot.GetLineFromLineNumber(0).GetText();
            var match = SearchOptionsRegex.Match(firstLine);
            if (match.Success)
            {
                var searchTerm = match.Groups["searchTerm"].Value;
                var matchCase = match.Groups["casing"].Success;
                var matchWholeWord = match.Groups["wholeWord"].Success;

                var regex = matchWholeWord ? string.Format(@"\b{0}\b", Regex.Escape(searchTerm)) : Regex.Escape(searchTerm);
                var casing = matchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                searchTextRegex = new Regex(regex, RegexOptions.None | casing);

                return true;
            }

            return false;
        }

        private static IEnumerable<ClassificationSpan> GetMatches(string text, Regex regex, SnapshotPoint snapStart, IClassificationType classificationType)
        {
            return from match in regex.Matches(text).Cast<Match>()
                   select new ClassificationSpan(new SnapshotSpan(snapStart + match.Index, match.Length), classificationType);
        }

        private IClassificationType SearchTermClassificationType
        {
            get { return classificationRegistry.GetClassificationType(OutputClassificationDefinitions.FindResultsSearchTerm); }
        }

        private IClassificationType FilenameClassificationType
        {
            get { return classificationRegistry.GetClassificationType(OutputClassificationDefinitions.FindResultsFilename); }
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}
