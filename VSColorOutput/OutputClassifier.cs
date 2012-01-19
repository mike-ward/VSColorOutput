// Copyright (c) 2012 Blue Onion Software, All rights reserved
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
        private bool _settingsLoaded;
        private IEnumerable<Classifier> _classifiers;
        private readonly StopOnFirstBuildError _stopOnFirstBuildError;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistry;
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public OutputClassifier(IClassificationTypeRegistryService registry, IServiceProvider serviceProvider)
        {
            _classificationTypeRegistry = registry;
            _stopOnFirstBuildError = new StopOnFirstBuildError(serviceProvider);
        }

        private struct Classifier
        {
            public string Type { get; set; }
            public Predicate<string> Test { get; set; }
        }

        public void ClearSettings()
        {
            _settingsLoaded = false;
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
                var snapshotSpan = new SnapshotSpan(line.Start, line.Length);
                var text = line.Snapshot.GetText(snapshotSpan);
                if (string.IsNullOrEmpty(text) == false)
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
            if (_settingsLoaded == false)
            {
                var settings = new Settings();
                settings.Load();
                var patterns = settings.Patterns ?? new RegExClassification[0];
                var classifiers =
                    (from pattern in patterns
                     let test = new Regex(pattern.RegExPattern, pattern.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None)
                     select new Classifier
                     {
                         Type = pattern.ClassificationType.ToString(),
                         Test = text => test.IsMatch(text)
                     }).ToList();
                classifiers.Add(new Classifier
                {
                    Type = OutputClassificationDefinitions.BuildText,
                    Test = t => true
                });
                _classifiers = classifiers;
                _stopOnFirstBuildError.Enabled = settings.EnableStopOnBuildError;
                _settingsLoaded = true;
            }
        }
    }
}