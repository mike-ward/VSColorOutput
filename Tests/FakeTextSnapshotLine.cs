using System;
using Microsoft.VisualStudio.Text;

namespace Tests
{
    public class FakeTextSnapshotLine : ITextSnapshotLine
    {
        private readonly string text;

        public FakeTextSnapshotLine(ITextSnapshot snapshot, string text, int position, int lineNumber)
        {
            Snapshot = snapshot;
            this.text = text;
            LineNumber = lineNumber;
            Start = new SnapshotPoint(snapshot, position);
        }

        public string GetText()
        {
            return text;
        }

        public string GetTextIncludingLineBreak()
        {
            return text + Environment.NewLine;
        }

        public string GetLineBreakText()
        {
            return Environment.NewLine;
        }

        public ITextSnapshot Snapshot { get; private set; }

        public SnapshotSpan Extent
        {
            get { throw new NotImplementedException(); }
        }

        public SnapshotSpan ExtentIncludingLineBreak
        {
            get { throw new NotImplementedException(); }
        }

        public int LineNumber { get; private set; }

        public SnapshotPoint Start { get; private set; }

        public int Length
        {
            get { return text.Length; }
        }

        public int LengthIncludingLineBreak
        {
            get { return text.Length + LineBreakLength; }
        }

        public SnapshotPoint End
        {
            get { return Start + Length; }
        }

        public SnapshotPoint EndIncludingLineBreak
        {
            get { return Start + LengthIncludingLineBreak; }
        }

        public int LineBreakLength
        {
            get { return Environment.NewLine.Length; }
        }
    }
}