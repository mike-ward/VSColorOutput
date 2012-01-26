using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BlueOnionSoftware
{
    [ContentType("code")]
    [Export(typeof(IClassifierProvider))]
    public class FindResultsClassifierProvider : IClassifierProvider
    {
        public static IClassifier FindResultsClassifier { get; private set; }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            return FindResultsClassifier ?? (FindResultsClassifier = new FindResultsClassifier());
        }
    }
}