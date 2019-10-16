using System;
using Microsoft.VisualStudio.Text;

namespace Tests
{
    public class FakeTextSnapshotLine : ITextSnapshotLine
    {
        private readonly string _text;

        public FakeTextSnapshotLine(ITextSnapshot snapshot, string text, int position, int lineNumber)
        {
            Snapshot = snapshot;
            this._text = text;
            LineNumber = lineNumber;
            Start = new SnapshotPoint(snapshot, position);
        }

        public string GetText()
        {
            return _text;
        }

        public string GetTextIncludingLineBreak()
        {
            return _text + Environment.NewLine;
        }

        public string GetLineBreakText()
        {
            return Environment.NewLine;
        }

        public ITextSnapshot Snapshot { get; }

        public SnapshotSpan Extent
        {
            get { throw new NotImplementedException(); }
        }

        public SnapshotSpan ExtentIncludingLineBreak
        {
            get { throw new NotImplementedException(); }
        }

        public int LineNumber { get; }

        public SnapshotPoint Start { get; }

        public int Length => _text.Length;

        public int LengthIncludingLineBreak => _text.Length + LineBreakLength;

        public SnapshotPoint End => Start + Length;

        public SnapshotPoint EndIncludingLineBreak => Start + LengthIncludingLineBreak;

        public int LineBreakLength => Environment.NewLine.Length;
  }
}