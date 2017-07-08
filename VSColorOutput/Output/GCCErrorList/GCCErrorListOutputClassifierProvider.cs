using System;
using System.ComponentModel.Composition;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using VSColorOutput.State;

namespace VSColorOutput.Output.GCCErrorList
{
    [ContentType("output")]
    [Export(typeof(IClassifierProvider))]
    class GCCErrorListOutputClassifierProvider : IClassifierProvider
    {
        private static GCCErrorListOutputClassifier _outputClassifier;

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            try
            {
                if (_outputClassifier == null)
                {
                    Interlocked.CompareExchange(
                        ref _outputClassifier,
                        new GCCErrorListOutputClassifier(),
                        null);
                    GCCErrorGenerator.Initialize(ServiceProvider.GlobalProvider);
                    _outputClassifier.Initialize();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                throw;
            }
            return _outputClassifier;
        }
    }
}
