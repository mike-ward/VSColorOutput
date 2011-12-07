// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

#pragma warning disable 67

namespace BlueOnionSoftware
{
    public class OutputClassifier : IClassifier
    {
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistry;

        public OutputClassifier(IClassificationTypeRegistryService registry)
        {
            _classificationTypeRegistry = registry;
        }

        private static readonly Regex _containsInfo = new Regex(@"(\W|^)information\W", RegexOptions.IgnoreCase);
        private static readonly Regex _containsWarn = new Regex(@"(\W|^)warning\W", RegexOptions.IgnoreCase);
        private static readonly Regex _containsError = new Regex(@"(\W|^)(error|fail|failed|exception)\W", RegexOptions.IgnoreCase);

        private struct Classifier
        {
            public string Name { get; set; }
            public Predicate<string> Test { get; set; }
        }

        private static readonly Classifier[] _classifiers = new []
        {
            new Classifier {Test = text => text.Contains("+++>"), Name = OutputClassificationDefinitions.LogSpecial},
            new Classifier {Test = text => text.Contains("----"), Name = OutputClassificationDefinitions.BuildHead},
            new Classifier {Test = text => text.Contains("===="), Name = OutputClassificationDefinitions.BuildHead},
            new Classifier {Test = text => text.Contains("0 failed,"), Name = OutputClassificationDefinitions.BuildHead},
            new Classifier {Test = text => _containsInfo.IsMatch(text), Name = OutputClassificationDefinitions.LogInfo},
            new Classifier {Test = text => _containsWarn.IsMatch(text), Name = OutputClassificationDefinitions.LogWarn},
            new Classifier {Test = text => _containsError.IsMatch(text), Name = OutputClassificationDefinitions.LogError},
            new Classifier {Test = text => true, Name = OutputClassificationDefinitions.BuildText}
        };

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var spans = new List<ClassificationSpan>();
            var snapshot = span.Snapshot;
            if (snapshot == null || snapshot.Length == 0)
            {
                return spans;
            }
            var start = span.Start.GetContainingLine().LineNumber;
            var end = (span.End - 1).GetContainingLine().LineNumber;
            for (var i = start; i <= end; i++)
            {
                var line = snapshot.GetLineFromLineNumber(i);
                var snapshotSpan = new SnapshotSpan(line.Start, Math.Min(150, line.Length));
                var text = line.Snapshot.GetText(snapshotSpan);
                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    var classificationName = _classifiers.First(t => t.Test(text)).Name;
                    var type = _classificationTypeRegistry.GetClassificationType(classificationName);
                    spans.Add(new ClassificationSpan(line.Extent, type));
                }
            }
            return spans;
        }
    }
}