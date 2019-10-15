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
        private string _text;
        private FormattedText _formattedText;
        private double _verticalOffset;
        private double _horizontalOffset;

        public object LineTag { get; private set; }

        public TimeStampVisual() => SnapsToDevicePixels = true;

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawText(_formattedText, new Point(_horizontalOffset, _verticalOffset));
        }

        internal void Update(
            string text,
            ITextViewLine line,
            IWpfTextView view,
            TextRunProperties formatting,
            double marginWidth,
            double verticalOffset)
        {
            LineTag = line.IdentityTag;

            if (_text == null || !string.Equals(_text, text, StringComparison.Ordinal))
            {
                _text = text;
                var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
                _formattedText = new FormattedText(
                    _text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    formatting.Typeface,
                    formatting.FontRenderingEmSize,
                    formatting.ForegroundBrush,
                    pixelsPerDip);

                _horizontalOffset = Math.Round(marginWidth - _formattedText.Width);
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