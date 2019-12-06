using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using VSColorOutput.Output.BuildEvents;
using VSColorOutput.Output.ColorClassifier;
using VSColorOutput.State;

namespace VSColorOutput.Output.TimeStamp
{
    public sealed class TimeStampMargin : Canvas, IWpfTextViewMargin
    {
        private readonly IWpfTextView _textView;
        private readonly IClassificationFormatMap _formatMap;
        private readonly IClassificationType _timestampClassification;
        private readonly Canvas _translatedCanvas = new Canvas();
        private readonly List<DateTime> _lineTimeStamps = new List<DateTime>();

        private string _elapsedTimeFormat = Settings.DefaultTimeStampFormat;
        private string _differenceTimeFormat = Settings.DefaultTimeStampFormat;

        private bool _disposed;
        private TextRunProperties _textRunProperties;
        private double _oldViewportTop = double.MinValue;

        public bool Enabled { get; } = true;
        public double MarginSize => ActualHeight;
        public FrameworkElement VisualElement => this;

        public ITextViewMargin GetTextViewMargin(string marginName) => marginName == nameof(TimeStampMargin) ? this : null;

        public TimeStampMargin(IWpfTextView textView, TimeStampMarginProvider timeStampMarginProvider)
        {
            _textView = textView;

            _formatMap = timeStampMarginProvider.ClassificationFormatMappingService
                .GetClassificationFormatMap(_textView);

            _timestampClassification = timeStampMarginProvider.ClassificationTypeRegistryService
                .GetClassificationType(ClassificationTypeDefinitions.TimeStamp);

            ClipToBounds = true;
            IsHitTestVisible = false;
            Children.Add(_translatedCanvas);
            TextOptions.SetTextHintingMode(this, TextHintingMode.Fixed);

            IsVisibleChanged += OnVisibleChanged;
            _textView.TextBuffer.Changed += TextBufferOnChanged;
            Settings.SettingsUpdated += OnSettingsOnSettingsUpdated;
            UpdateFormatMap();
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) _textView.LayoutChanged += TextViewOnLayoutChanged;
            else _textView.LayoutChanged -= TextViewOnLayoutChanged;
        }

        private void TextViewOnLayoutChanged(object sender, TextViewLayoutChangedEventArgs textViewLayoutChangedEventArgs)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_oldViewportTop != _textView.ViewportTop)
            {
                _oldViewportTop = _textView.ViewportTop;
                _translatedCanvas.RenderTransform = new TranslateTransform(0.0, -_textView.ViewportTop);
            }
            Update();
        }

        private void OnSettingsOnSettingsUpdated(object sender, EventArgs args)
        {
            UpdateFormatMap();
        }

        private void TextBufferOnChanged(object sender, TextContentChangedEventArgs ea)
        {
            Func<int, DateTime, IEnumerable<DateTime>> fill = (count, time) => Enumerable.Range(0, count).Select(t => time);

            foreach (var textChange in ea.Changes)
            {
                var lineNumber = ea.Before.GetLineFromPosition(textChange.OldPosition).LineNumber;

                if (textChange.LineCountDelta > 0)
                {
                    var count = _lineTimeStamps.Count;
                    _lineTimeStamps.InsertRange(
                        Math.Min(lineNumber, count),
                        fill(textChange.LineCountDelta + lineNumber - count, DateTime.Now));
                }
                else if (textChange.LineCountDelta < 0)
                {
                    try
                    {
                        _lineTimeStamps.RemoveRange(lineNumber, -textChange.LineCountDelta);
                    }
                    catch (ArgumentException)
                    {
                        _lineTimeStamps.Clear();
                    }
                }
            }
        }

        private void Update()
        {
            var hashSet = new HashSet<object>();
            foreach (var textViewLine in _textView.TextViewLines.Where(textViewLine => textViewLine.IsFirstTextViewLineForSnapshotLine))
            {
                hashSet.Add(textViewLine.IdentityTag);
            }

            var dictionary = new Dictionary<object, TimeStampVisual>();
            var list1 = new List<int>();
            for (var index = _translatedCanvas.Children.Count - 1; index >= 0; --index)
            {
                var timeStampVisual = _translatedCanvas.Children[index] as TimeStampVisual;
                if (timeStampVisual != null)
                {
                    if (hashSet.Contains(timeStampVisual.LineTag)) dictionary.Add(timeStampVisual.LineTag, timeStampVisual);
                    else list1.Add(index);
                }
            }

            var list2 = new List<TimeStampVisual>();
            foreach (var line in _textView.TextViewLines)
            {
                if (line.IsFirstTextViewLineForSnapshotLine)
                {
                    var lineNumber = line.Start.GetContainingLine().LineNumber;
                    if (lineNumber < _lineTimeStamps.Count)
                    {
                        var timeStamp = _lineTimeStamps[lineNumber];
                        var previousTimeStamp = _lineTimeStamps[Math.Max(lineNumber - 1, 0)];
                        TimeStampVisual timeStampVisual;
                        if (!dictionary.TryGetValue(line.IdentityTag, out timeStampVisual))
                        {
                            var index = list1.Count - 1;
                            if (index < 0)
                            {
                                timeStampVisual = new TimeStampVisual();
                                list2.Add(timeStampVisual);
                            }
                            else
                            {
                                timeStampVisual = _translatedCanvas.Children[list1[index]] as TimeStampVisual;
                                list1.RemoveAt(index);
                            }
                        }

                        if (timeStampVisual != null)
                        {
                            var startDiff = timeStamp - BuildEventsProvider.BuildEvents.DebugStartTime;
                            var lastDiff = timeStamp - previousTimeStamp;
                            var text = string.Empty;

                            try
                            {
                                text = lineNumber == 0 || lastDiff != TimeSpan.Zero
                                    ? TimeText(startDiff, lastDiff)
                                    : "";
                            }
                            catch (Exception ex)
                            {
                                text = ex.Message;
                            }
                            timeStampVisual.Update(text, line, _textView, _textRunProperties, MinWidth, _oldViewportTop);
                        }
                    }
                }
            }

            foreach (var index in list1) _translatedCanvas.Children.RemoveAt(index);
            foreach (var element in list2) _translatedCanvas.Children.Add(element);
        }

        private void UpdateFormatMap()
        {
            var colorMap = ColorMap.GetMap();
            var textProperties = _formatMap.GetTextProperties(_timestampClassification);
            var color = colorMap[ClassificationTypeDefinitions.TimeStamp];
            var wpfColor = ClassificationTypeDefinitions.ToMediaColor(color);
            textProperties = textProperties.SetForeground(wpfColor);

            _formatMap.SetTextProperties(_timestampClassification, textProperties);
            _textRunProperties = textProperties;
            _translatedCanvas.Children.Clear();

            Background = _textRunProperties.BackgroundBrush;
            MinWidth = CalculateMarginWidth();
            Update();
        }

        private string TimeText(TimeSpan elapsed, TimeSpan difference)
        {
            try
            {
                var elapsedText = string.IsNullOrWhiteSpace(_elapsedTimeFormat) ? string.Empty : elapsed.ToString(_elapsedTimeFormat);
                var differenceText = string.IsNullOrWhiteSpace(_differenceTimeFormat) ? string.Empty : difference.ToString(_differenceTimeFormat);

                return elapsedText
                    + (string.IsNullOrWhiteSpace(differenceText) ? string.Empty : $" {differenceText}");
            }
            catch (Exception)
            {
                return "invalid timespan format";
            }
        }

        private double CalculateMarginWidth()
        {
            var settings = Settings.Load();
            if (settings.ShowTimeStamps == false) return 0;
            _elapsedTimeFormat = settings.TimeStampElapsed;
            _differenceTimeFormat = settings.TimeStampDifference;
            var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            var timespan = TimeSpan.FromHours(12.123);
            var sampleTimeText = TimeText(timespan, timespan);

            var text = new FormattedText(
                sampleTimeText,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                _textRunProperties.Typeface,
                _textRunProperties.FontRenderingEmSize,
                Brushes.Black,
                pixelsPerDip);

            return text.Width + 3;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _textView.TextBuffer.Changed -= TextBufferOnChanged;
            IsVisibleChanged -= OnVisibleChanged;
            Settings.SettingsUpdated -= OnSettingsOnSettingsUpdated;
        }
    }
}