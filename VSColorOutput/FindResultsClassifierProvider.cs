using System.ComponentModel.Composition;
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
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry;

        public static IClassifier FindResultsClassifier { get; private set; }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            return FindResultsClassifier ?? (FindResultsClassifier = new FindResultsClassifier(ClassificationRegistry));
        }
    }
}