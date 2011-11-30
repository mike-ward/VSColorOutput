// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 649

namespace BlueOnionSoftware
{
    [ContentType("BuildOutput")]
    [ContentType("DebugOutput")]
    [Export(typeof(IClassifierProvider))]
    public class OutputClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry;

        private static OutputClassifier _outputClassifier;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (_outputClassifier == null)
            {
                _outputClassifier = new OutputClassifier(ClassificationRegistry);
            }
            return _outputClassifier;
        }
    }
}