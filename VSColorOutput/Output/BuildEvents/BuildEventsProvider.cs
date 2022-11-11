using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using VSColorOutput.State;

#pragma warning disable 649

namespace VSColorOutput.Output.BuildEvents
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("output")]
    public class BuildEventsProvider : IClassifierProvider
    {
        [Import]
        internal SVsServiceProvider ServiceProvider;

        private static BuildEvents _buildEvents;
        public static BuildEvents BuildEvents => _buildEvents;

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            try
            {
                if (_buildEvents == null)
                {
                    Interlocked.CompareExchange(
                        ref _buildEvents,
                        new BuildEvents(),
                        null);

                    _buildEvents.Initialize(ServiceProvider);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                throw;
            }

            return null;
        }
    }
}