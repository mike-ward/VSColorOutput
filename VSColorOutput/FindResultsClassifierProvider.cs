using System.ComponentModel.Composition;
using System.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

// Unassigned variables
#pragma warning disable 649

namespace BlueOnionSoftware
{
    [ContentType("FindResults")]
    [Export(typeof(IClassifierProvider))]
    public class FindResultsClassifierProvider : IClassifierProvider
    {
        [Import] internal IClassificationTypeRegistryService ClassificationRegistry;
        [Import] internal IClassificationFormatMapService ClassificationFormatMapService;

        private static FindResultsClassifier _findResultsClassifier;

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (_findResultsClassifier == null)
            {
                Interlocked.CompareExchange(
                    ref _findResultsClassifier,
                    new FindResultsClassifier(ClassificationRegistry, ClassificationFormatMapService), 
                    null);
            }
            return _findResultsClassifier;
        }
    }
}