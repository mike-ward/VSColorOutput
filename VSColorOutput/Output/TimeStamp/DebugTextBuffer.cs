using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace VSColorOutput.Output.TimeStamp
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("output")]
    public class DebugTextBuffer : IClassifierProvider
    {
        private static bool _initialized;

        [Import] internal ITextBufferFactoryService TextBufferFactoryService { get; private set; }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            if (!_initialized)
            {
                _initialized = true;
                TextBufferFactoryService.TextBufferCreated += (sender, args) =>
                {
                    System.Diagnostics.Debug.WriteLine(args.TextBuffer.ContentType.DisplayName);
                    if (args.TextBuffer.ContentType.DisplayName.Equals("DebugOutput", StringComparison.OrdinalIgnoreCase))
                    {
                        System.Diagnostics.Debug.WriteLine(args.TextBuffer.ContentType.DisplayName);
                    }
                };
            }
            return null;
        }
    }
}