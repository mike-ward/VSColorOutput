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

namespace VSColorOutput.Output.TimeStamp
{
    public sealed class TimeStampMargin : Canvas, IWpfTextViewMargin
    {
        private bool _disposed;
        private readonly IWpfTextView _textView;
        private double _oldViewportTop = double.MinValue;
        private readonly Canvas _translatedCanvas = new Canvas();
        private readonly List<DateTime> _lineTimeStamps = new List<DateTime>();
        private TextRunProperties _formatting;
        private readonly IClassificationFormatMap _formatMap;
        private readonly IClassificationType _lineNumberClassificaiton;

        public double MarginSize => ActualHeight;
        public bool Enabled { get; } = true;
        public FrameworkElement VisualElement => this;
        public ITextViewMargin GetTextViewMargin(string marginName) => marginName == nameof(TimeStampMargin) ? this : null;

        public TimeStampMargin(IWpfTextView textView, TimeStampMarginProvider timeStampMarginProvider)
        {
            _textView = textView;
            _textView.TextBuffer.Changed += TextBufferOnChanged;
            _formatMap = timeStampMarginProvider.ClassificationFormatMappingService.GetClassificationFormatMap(_textView);
            _lineNumberClassificaiton = timeStampMarginProvider.ClassificationTypeRegistryService.GetClassificationType("line number");
            IsVisibleChanged += OnVisibleChanged;

            ClipToBounds = true;
            IsHitTestVisible = false;
            Children.Add(_translatedCanvas);
            TextOptions.SetTextHintingMode(this, TextHintingMode.Fixed);
        }

        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                _textView.LayoutChanged += TextViewOnLayoutChanged;
                _formatMap.ClassificationFormatMappingChanged += OnClassificationFormatMappingChanged;
                SetFontFromClassification();
            }
            else
            {
                _textView.LayoutChanged -= TextViewOnLayoutChanged;
                _formatMap.ClassificationFormatMappingChanged -= OnClassificationFormatMappingChanged;
            }
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            SetFontFromClassification();
        }

        private void TextViewOnLayoutChanged(object sender, TextViewLayoutChangedEventArgs textViewLayoutChangedEventArgs)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_oldViewportTop != _textView.ViewportTop)
            {
                _oldViewportTop = _textView.ViewportTop;
                _translatedCanvas.RenderTransform = new TranslateTransform(0.0, -_textView.ViewportTop);
            }
            UpdateLineNumbers();
        }

        private void TextBufferOnChanged(object sender, TextContentChangedEventArgs ea)
        {
            foreach (var textChange in ea.Changes)
            {
                if (textChange.LineCountDelta > 0)
                {
                    var times = Enumerable.Range(0, textChange.LineCountDelta).Select(t => DateTime.Now);
                    _lineTimeStamps.InsertRange(ea.Before.GetLineFromPosition(textChange.OldPosition).LineNumber, times.ToList());
                }
                else if (textChange.LineCountDelta < 0)
                {
                    _lineTimeStamps.RemoveRange(ea.Before.GetLineFromPosition(textChange.OldPosition).LineNumber, -textChange.LineCountDelta);
                }
            }
        }

        private void UpdateLineNumbers()
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
                        timeStampVisual?.UpdateVisual(timeStamp, line, _textView, _formatting,
                            MinWidth, _oldViewportTop, false, true);
                    }
                }
            }
            foreach (var index in list1) _translatedCanvas.Children.RemoveAt(index);
            foreach (var element in list2) _translatedCanvas.Children.Add(element);
        }

        private void SetFontFromClassification()
        {
            var textProperties = _formatMap.GetTextProperties(_lineNumberClassificaiton);
            var brush = textProperties.BackgroundBrush;
            if (brush.Opacity < 1.0)
            {
                brush = brush.Clone();
                brush.Opacity = 1.0;
                brush.Freeze();
                textProperties = textProperties.SetBackgroundBrush(brush);
            }
            Background = brush;
            _formatting = textProperties;
            _translatedCanvas.Children.Clear();
            MinWidth = CalculateMarginWidth();
            UpdateLineNumbers();
        }

        private double CalculateMarginWidth()
        {
            return new FormattedText(
                "MM:MM.MMM",
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                _formatting.Typeface,
                _formatting.FontRenderingEmSize,
                Brushes.Black).Width;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _textView.TextBuffer.Changed -= TextBufferOnChanged;
        }
    }
}