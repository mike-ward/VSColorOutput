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

        private struct Classifier
        {
            public string Type { get; set; }
            public Predicate<string> Test { get; set; }
        }

        private IEnumerable<Classifier> _classifiers;

        public void ClearClassifiers()
        {
            _classifiers = null;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var spans = new List<ClassificationSpan>();
            var snapshot = span.Snapshot;
            if (snapshot == null || snapshot.Length == 0)
            {
                return spans;
            }
            LoadClassifiers();
            var start = span.Start.GetContainingLine().LineNumber;
            var end = (span.End - 1).GetContainingLine().LineNumber;
            for (var i = start; i <= end; i++)
            {
                var line = snapshot.GetLineFromLineNumber(i);
                var snapshotSpan = new SnapshotSpan(line.Start, Math.Min(150, line.Length));
                var text = line.Snapshot.GetText(snapshotSpan);
                if (string.IsNullOrWhiteSpace(text) == false)
                {
                    var classificationName = _classifiers.First(classifier => classifier.Test(text)).Type;
                    var type = _classificationTypeRegistry.GetClassificationType(classificationName);
                    spans.Add(new ClassificationSpan(line.Extent, type));
                }
            }
            return spans;
        }

        private void LoadClassifiers()
        {
            if (_classifiers == null)
            {
                var patterns = Settings.LoadPatterns() ?? new RegExClassification[0];
                var classifiers = new List<Classifier>();
                foreach (var pattern in patterns)
                {
                    var test = new Regex(pattern.RegExPattern, pattern.IgnoreCase 
                        ? RegexOptions.IgnoreCase 
                        : RegexOptions.None);
                    classifiers.Add(new Classifier
                    {
                        Type = pattern.ClassificationType.ToString(),
                        Test = text => test.IsMatch(text)
                    });
                }
                classifiers.Add(new Classifier
                {
                    Type = OutputClassificationDefinitions.BuildText,
                    Test = t => true
                });
                _classifiers = classifiers;
            }
        }
    }
}