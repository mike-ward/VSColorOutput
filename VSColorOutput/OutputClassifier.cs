using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

// ReSharper disable EmptyGeneralCatchClause
#pragma warning disable 67

namespace BlueOnionSoftware
{
    public class OutputClassifier : IClassifier
    {
        private IList<Classifier> _classifiers;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistry;
        private readonly IClassificationFormatMapService _formatMapService;
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public OutputClassifier(IClassificationTypeRegistryService registry, IClassificationFormatMapService formatMapService)
        {
            try
            {
                _classificationTypeRegistry = registry;
                _formatMapService = formatMapService;

                Settings.SettingsUpdated += (sender, args) =>
                {
                    UpdateClassifiers();
                    UpdateFormatMap();
                };
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                throw;
            }
        }

        private struct Classifier
        {
            public string Type { get; set; }
            public Predicate<string> Test { get; set; }
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            try
            {
                var spans = new List<ClassificationSpan>();
                var snapshot = span.Snapshot;
                if (snapshot == null || snapshot.Length == 0) return spans;
                if (_classifiers == null) UpdateClassifiers();

                var classifiers = _classifiers ?? new List<Classifier>();
                var start = span.Start.GetContainingLine().LineNumber;
                var end = (span.End - 1).GetContainingLine().LineNumber;
                for (var i = start; i <= end; i++)
                {
                    var line = snapshot.GetLineFromLineNumber(i);
                    var snapshotSpan = new SnapshotSpan(line.Start, line.Length);
                    var text = line.Snapshot.GetText(snapshotSpan);

                    if (string.IsNullOrEmpty(text) == false)
                    {
                        var classificationName = classifiers.First(classifier => classifier.Test(text)).Type;
                        var type = _classificationTypeRegistry.GetClassificationType(classificationName);
                        spans.Add(new ClassificationSpan(line.Extent, type));
                    }
                }
                return spans;
            }
            catch (RegexMatchTimeoutException)
            {
                // eat it.
                return new List<ClassificationSpan>();
            }
            catch (Exception ex)
            {
                LogError(ex.ToString());
                throw;
            }
        }

        private void UpdateClassifiers()
        {
            var settings = Settings.Load();
            var patterns = settings.Patterns ?? new RegExClassification[0];

            var classifiers = patterns.Select(
                pattern => new
                {
                    classificationType = pattern.ClassificationType.ToString(),
                    test = RegExClassification.RegExFactory(pattern)
                })
                .Select(temp => new Classifier
                {
                    Type = temp.classificationType,
                    Test = text => temp.test.IsMatch(text)
                })
                .ToList();

            classifiers.Add(new Classifier
            {
                Type = OutputClassificationDefinitions.BuildText,
                Test = t => true
            });

            _classifiers = classifiers;
        }

        public void UpdateFormatMap()
        {
            var colorMap = ColorMap.GetMap();
            var formatMap = _formatMapService.GetClassificationFormatMap("output");
            foreach (var category in OutputClassificationDefinitions.Categories)
            {
                var classificationType = _classificationTypeRegistry.GetClassificationType(category);
                var textProperties = formatMap.GetTextProperties(classificationType);
                var color = colorMap[category];
                var wpfColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                formatMap.SetTextProperties(classificationType, textProperties.SetForeground(wpfColor));
            }
        }

        public static void LogError(string message)
        {
            try
            {
                // I'm co-opting the Visual Studio event source because I can't register
                // my own from a .VSIX installer.
                EventLog.WriteEntry("Microsoft Visual Studio",
                    "VSColorOutput: " + (message ?? "null"),
                    EventLogEntryType.Error);
            }
            catch
            {
                // Don't kill extension for logging errors
            }
        }
    }
}