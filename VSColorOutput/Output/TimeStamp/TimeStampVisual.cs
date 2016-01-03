using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace VSColorOutput.Output.TimeStamp
{
    public class TimeStampVisual : UIElement
    {
        private FormattedText _text;
        private DateTime _timeStamp;
        private double _verticalOffset;
        private double _horizontalOffset;

        public object LineTag { get; private set; }
        public const string StartingTimeStamp = "00:00:000 (00:00:000)";

        public TimeStampVisual()
        {
            SnapsToDevicePixels = true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawText(_text, new Point(_horizontalOffset, _verticalOffset));
        }

        internal void Update(
            bool first,
            DateTime debugStartTime,
            DateTime timeStamp,
            DateTime previousTimeStamp,
            ITextViewLine line,
            IWpfTextView view,
            TextRunProperties formatting,
            double marginWidth,
            double verticalOffset)
        {
            LineTag = line.IdentityTag;

            if (timeStamp != _timeStamp)
            {
                _timeStamp = timeStamp;
                var startDiff = timeStamp - debugStartTime;
                var lastDiff = timeStamp - previousTimeStamp;

                var text = first || lastDiff != TimeSpan.Zero
                    ? $"{startDiff.Minutes:D2}:{startDiff.Seconds:D2}.{startDiff.Milliseconds:D3} " +
                        $"({lastDiff.Minutes:D2}:{lastDiff.Seconds:D2}.{lastDiff.Milliseconds:D3})"
                    : "";

                _text = new FormattedText(
                    text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    formatting.Typeface,
                    formatting.FontRenderingEmSize,
                    formatting.ForegroundBrush);

                _horizontalOffset = Math.Round(marginWidth - _text.Width);
                InvalidateVisual();
            }

            var num = line.TextTop - view.ViewportTop + verticalOffset;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (num == _verticalOffset) return;
            _verticalOffset = num;
            InvalidateVisual();
        }
    }
}