using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using VSColorOutput.Output.ColorClassifier;
using VSColorOutput.State;

namespace VSColorOutput.Output.GCCErrorList
{
    class GCCErrorListOutputClassifier : IClassifier
    {
        private int _initialized;
        private IList<Classifier> _classifiers;
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public void Initialize()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 1) return;
            try
            {
                Settings.SettingsUpdated += (sender, args) =>
                {
                    UpdateClassifiers();
                };
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                throw;
            }
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
                    if (line == null) continue;
                    var snapshotSpan = new SnapshotSpan(line.Start, line.Length);
                    var text = line.Snapshot.GetText(snapshotSpan);
                    if (string.IsNullOrEmpty(text)) continue;

                    var classificationName = classifiers.FirstOrDefault(classifier => classifier.Test(text)).Type;
                    if (classificationName == null)
                    {
                        continue;
                    }
                    switch (classificationName)
                    {
                        case ClassificationTypeDefinitions.LogError:
                            GCCErrorGenerator.AddError(GCCErrorListItem.Parse(text));
                            break;
                        case ClassificationTypeDefinitions.LogWarn:
                            GCCErrorGenerator.AddWarning(GCCErrorListItem.Parse(text));
                            break;
                        case ClassificationTypeDefinitions.LogInfo:
                            GCCErrorGenerator.AddMessage(GCCErrorListItem.Parse(text));
                            break;

                    }
                }
                return spans;
            }
            catch (FormatException)
            {
                // eat it.
                return new List<ClassificationSpan>();
            }
            catch (RegexMatchTimeoutException)
            {
                // eat it.
                return new List<ClassificationSpan>();
            }
            catch (NullReferenceException)
            {
                // eat it.    
                return new List<ClassificationSpan>();
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
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
                Type = ClassificationTypeDefinitions.BuildText,
                Test = t => true
            });

            _classifiers = classifiers;
        }

        protected virtual void OnClassificationChanged(ClassificationChangedEventArgs e)
        {
            ClassificationChanged?.Invoke(this, e);
        }
    }
}
