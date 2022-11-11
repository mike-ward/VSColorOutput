using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Threading;

// Unassigned variables
#pragma warning disable 649

namespace VSColorOutput.FindResults
{
    [ContentType("FindResults")]
    [Export(typeof(IClassifierProvider))]
    public class FindResultsClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry;

        [Import]
        internal IClassificationFormatMapService ClassificationFormatMapService;

        private static FindResultsClassifier _findResultsClassifier;

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (_findResultsClassifier == null)
            {
                Interlocked.CompareExchange(
                    ref _findResultsClassifier,
                    new FindResultsClassifier(),
                    null);

                _findResultsClassifier.Initialize(ClassificationRegistry, ClassificationFormatMapService);
            }

            return _findResultsClassifier;
        }
    }
}