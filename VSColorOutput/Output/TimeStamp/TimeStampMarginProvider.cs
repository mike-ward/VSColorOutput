using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace VSColorOutput.Output.TimeStamp
{
    [Name("TimeStampMargin")]
    [ContentType("DebugOutput")]
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Order(Before = PredefinedMarginNames.Spacer)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]   
    [MarginContainer(PredefinedMarginNames.LeftSelection)]
    public class TimeStampMarginProvider : IWpfTextViewMarginProvider
    {
        [Import] internal IClassificationFormatMapService ClassificationFormatMappingService { get; private set; }
        [Import] internal IClassificationTypeRegistryService ClassificationTypeRegistryService { get; private set; }

        public static bool Initialized { get; private set; }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            Initialized = true;
            return new TimeStampMargin(textViewHost.TextView, this);
        }
    }
}