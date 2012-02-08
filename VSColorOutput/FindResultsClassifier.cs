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
        private static Regex FilenameRegex;

        private SearchOptions searchOptions;
        private Regex searchTextRegex;

        static FindResultsClassifier()
        {
            // TODO: What about localised instances of VS?
            SearchOptionsRegex = new Regex("Find all \"(?<searchTerm>[^\"]+)\", (?<casing>Match case)?", RegexOptions.Compiled);
            FilenameRegex = new Regex(@"^(.*:.*\(\d+\):)", RegexOptions.Compiled);
        }

        public FindResultsClassifier(IClassificationTypeRegistryService classificationRegistry)
        {
            this.classificationRegistry = classificationRegistry;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var classifications = new List<ClassificationSpan>();

            var snapshot = span.Snapshot;
            if (snapshot == null || snapshot.Length == 0)
            {
                return classifications;
            }

            ResetSearchOptions(span);
            if (searchOptions != null)
            {
                var text = span.GetText();

                classifications.AddRange(GetMatches(text, searchTextRegex, span.Start, SearchTermClassificationType));
                //classifications.AddRange(GetMatches(text, FilenameRegex, span.Start, filenameClassificationType));
            }   
         
            return classifications;
        }

        private void ResetSearchOptions(SnapshotSpan span)
        {
            if (span.Start.Position == 0)
            {
                searchOptions = null;

                var firstLine = span.Snapshot.GetLineFromLineNumber(0).GetText();
                var match = SearchOptionsRegex.Match(firstLine);
                if (match.Success)
                {
                    searchOptions = new SearchOptions
                                        {
                                            SearchTerm = match.Groups["searchTerm"].Value,
                                            MatchCase = match.Groups["casing"].Success
                                        };
                    var casing = searchOptions.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                    searchTextRegex = new Regex(searchOptions.SearchTerm, RegexOptions.None | casing);
                }
            }
        }

        private IEnumerable<ClassificationSpan> GetMatches(string text, Regex regex, SnapshotPoint snapStart, IClassificationType classificationType)
        {
            return from match in regex.Matches(text).Cast<Match>()
                   select new ClassificationSpan(new SnapshotSpan(snapStart + match.Index, match.Length), classificationType);
        }

        private IClassificationType SearchTermClassificationType
        {
            get { return classificationRegistry.GetClassificationType(OutputClassificationDefinitions.FindResultsSearchTerm); }
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        private class SearchOptions
        {
            public string SearchTerm;
            public bool MatchCase;
            //public bool MatchWholeWord;
            //public bool UsingRegex;
            //public bool UsingWildcards;     // What the heck is this?
        }
    }
}
