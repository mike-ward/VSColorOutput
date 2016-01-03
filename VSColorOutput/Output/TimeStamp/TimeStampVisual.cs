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
        private double _verticalOffset;
        private double _horizontalOffset;
        private FormattedText _text;

        public object LineTag { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public TimeStampVisual()
        {
            SnapsToDevicePixels = true;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawText(_text, new Point(_horizontalOffset, _verticalOffset));
        }

        internal void UpdateVisual(
            DateTime timeStamp,
            ITextViewLine line,
            IWpfTextView view,
            TextRunProperties formatting,
            double marginWidth, double verticalOffset,
            bool showHours,
            bool showMilliseconds)
        {
            LineTag = line.IdentityTag;

            if (timeStamp != TimeStamp)
            {
                TimeStamp = timeStamp;

                var text = string.Format(CultureInfo.InvariantCulture, showMilliseconds
                    ? (showHours ? "{0,2}:{1:D2}:{2:D2}.{3:D3}" : "{1:D2}:{2:D2}.{3:D3}")
                    : (showHours ? "{0,2}:{1:D2}:{2:D2}" : "{1:D2}:{2:D2}"),
                    timeStamp.Hour, timeStamp.Minute, timeStamp.Second, timeStamp.Millisecond);

                _text = new FormattedText(
                    text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    formatting.Typeface ?? new Typeface("Consolas"),
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