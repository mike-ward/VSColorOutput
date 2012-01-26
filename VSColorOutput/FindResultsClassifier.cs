using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace BlueOnionSoftware
{
    public class FindResultsClassifier : IClassifier
    {
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            // Quick fix to stop unused instance compiler error. Don't worry, it won't last
            if (ClassificationChanged == null)
            {
            }

            return new List<ClassificationSpan>();
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}