using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;

namespace VSColorOutput.Output.TimeStamp
{
    public class TimeStampMargin : Canvas, IWpfTextViewMargin
    {
        private IWpfTextView _textView;
        private TimeStampMarginProvider _timeStampMarginProvider;

        public TimeStampMargin(IWpfTextView textView, TimeStampMarginProvider timeStampMarginProvider)
        {
            _textView = textView;
            _timeStampMarginProvider = timeStampMarginProvider;
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            throw new NotImplementedException();
        }

        public double MarginSize => ActualHeight;
        public bool Enabled { get; } = true;
        public FrameworkElement VisualElement => this;

        public void Dispose()
        {
        }
    }
}