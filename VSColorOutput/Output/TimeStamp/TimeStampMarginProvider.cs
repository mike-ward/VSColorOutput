using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace VSColorOutput.Output.TimeStamp
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name("TimeStampMargin")]
    [MarginContainer("LeftSelection")]
    [Order(Before = "Spacer")]
    [ContentType("DebugOutput")]
    [TextViewRole("INTERACTIVE")]   
    public class TimeStampMarginProvider : IWpfTextViewMarginProvider
    {
        public TimeStampMarginProvider()
        {
        }

        [Import]
        internal IClassificationFormatMapService ClassificationFormatMappingService { get; private set; }

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistryService { get; private set; }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            return new TimeStampMargin(textViewHost.TextView, this);
        }
    }
}