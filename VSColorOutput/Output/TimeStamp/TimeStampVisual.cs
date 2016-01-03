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
            double marginWidth, double verticalOffset)
        {
            LineTag = line.IdentityTag;

            if (timeStamp != _timeStamp)
            {
                _timeStamp = timeStamp;

                _text = new FormattedText(
                    $"{timeStamp.Minute:D2}:{timeStamp.Second:D2}.{timeStamp.Millisecond:D3}",
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