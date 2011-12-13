// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 649

namespace BlueOnionSoftware
{
    [ContentType("TestOutput")]
    [ContentType("BuildOutput")]
    [ContentType("DebugOutput")]
    [Export(typeof(IClassifierProvider))]
    public class OutputClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry;

        public static OutputClassifier OutputClassifier { get; private set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (OutputClassifier == null)
            {
                OutputClassifier = new OutputClassifier(ClassificationRegistry);
                TextManagerEvents.RegisterForTextManagerEvents();
            }
            return OutputClassifier;
        }
    }
}