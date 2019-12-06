using System;
using System.ComponentModel.Composition;
using System.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using VSColorOutput.State;

#pragma warning disable 649

namespace VSColorOutput.Output.ColorClassifier
{
    [ContentType("output")]
    [ContentType("plaintext")]
    [Export(typeof(IClassifierProvider))]
    public class OutputClassifierProvider : IClassifierProvider
    {
        [Import] internal IClassificationTypeRegistryService ClassificationRegistry;
        [Import] internal IClassificationFormatMapService ClassificationFormatMapService;

        private static OutputClassifier _outputClassifier;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (!WillSupplyClassifier(buffer))
            {
                return null;
            }
            try
            {
                if (_outputClassifier == null)
                {
                    Interlocked.CompareExchange(
                        ref _outputClassifier,
                        new OutputClassifier(),
                        null);

                    _outputClassifier.Initialize(ClassificationRegistry, ClassificationFormatMapService);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                throw;
            }
            return _outputClassifier;
        }

        // This works around weird behavior of Visual Studio - when output window content
        // is saved to disk (using Ctrl+S) the ITextBuffer.ContentType is changed to "plaintext"
        // and it remains that way until Visual Studio is restarted.
        // The workaround tags each "output" so it can be identified after the content type is changed.
        private bool WillSupplyClassifier(ITextBuffer buffer)
        {
            var bufferProperties = buffer.Properties;
            if (bufferProperties.ContainsProperty(SuppliedClassifierForThisBufferKey))
            {
                // We worked with this object before so we can work with it once again.
                return true;
            }
            var contentType = buffer.ContentType;
            if (contentType.IsOfType("Output"))
            {
                bufferProperties.AddProperty(SuppliedClassifierForThisBufferKey, null);
                return true;
            }
            return false;
        }

        private const string SuppliedClassifierForThisBufferKey = "VSColorOutput.Output.SuppliedClassifierForThisTextBufferKey";
    }
}