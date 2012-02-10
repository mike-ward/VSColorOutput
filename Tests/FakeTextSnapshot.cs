using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace Tests
{
    public class FakeTextSnapshot : ITextSnapshot
    {
        private readonly string text;
        private readonly string[] lines;

        public FakeTextSnapshot(string text)
        {
            this.text = text;
            lines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            Lines = lines.Select((t, i) => GetLineFromLineNumber(i)).ToList();

            TextBuffer = new FakeTextBuffer();
            Version = new FakeVersion(1);
        }

        public string GetText(Span span)
        {
            return text.Substring(span.Start, span.Length);
        }

        public string GetText(int startIndex, int length)
        {
            return text.Substring(startIndex, text.Length);
        }

        public string GetText()
        {
            return text;
        }

        public char[] ToCharArray(int startIndex, int length)
        {
            return GetText(startIndex, length).ToCharArray();
        }

        public ITextSnapshotLine GetLineFromLineNumber(int lineNumber)
        {
            var position = GetPositionFromLineNumber(lineNumber);
            return new FakeTextSnapshotLine(this, lines[lineNumber], position, lineNumber);
        }

        public ITextSnapshotLine GetLineFromPosition(int position)
        {
            var lineNumber = GetLineNumberFromPosition(position);
            return new FakeTextSnapshotLine(this, lines[lineNumber], position, lineNumber);
        }

        public int GetLineNumberFromPosition(int position)
        {
            var count = 0;
            for (var i = 0; i < LineCount; i++)
            {
                count += lines[i].Length + Environment.NewLine.Length;
                if (count > position)
                    return i;
            }

            throw new ArgumentOutOfRangeException();
        }

        private int GetPositionFromLineNumber(int lineNumber)
        {
            var position = 0;
            for (var i = 0; i < lineNumber; i++)
                position += lines[i].Length + Environment.NewLine.Length;
            return position;
        }

        public int Length
        {
            get { return text.Length; }
        }

        public int LineCount
        {
            get { return lines.Length; }
        }

        public char this[int position]
        {
            get { return text[position]; }
        }

        public IEnumerable<ITextSnapshotLine> Lines { get; private set; }
        public ITextBuffer TextBuffer { get; private set; }
        public ITextVersion Version { get; private set; }

        #region Not required or implemented

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            throw new NotImplementedException();
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public void Write(TextWriter writer, Span span)
        {
            throw new NotImplementedException();
        }

        public void Write(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public IContentType ContentType
        {
            get { throw new NotImplementedException(); }
        }

        // Only here so that ToString will work in SnapshotSpan for debugging
        private class FakeTextBuffer : ITextBuffer
        {
            public FakeTextBuffer()
            {
                Properties = new PropertyCollection();
            }

            public PropertyCollection Properties { get; private set; }

            public ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag)
            {
                throw new NotImplementedException();
            }

            public ITextEdit CreateEdit()
            {
                throw new NotImplementedException();
            }

            public IReadOnlyRegionEdit CreateReadOnlyRegionEdit()
            {
                throw new NotImplementedException();
            }

            public void TakeThreadOwnership()
            {
                throw new NotImplementedException();
            }

            public bool CheckEditAccess()
            {
                throw new NotImplementedException();
            }

            public void ChangeContentType(IContentType newContentType, object editTag)
            {
                throw new NotImplementedException();
            }

            public ITextSnapshot Insert(int position, string text)
            {
                throw new NotImplementedException();
            }

            public ITextSnapshot Delete(Span deleteSpan)
            {
                throw new NotImplementedException();
            }

            public ITextSnapshot Replace(Span replaceSpan, string replaceWith)
            {
                throw new NotImplementedException();
            }

            public bool IsReadOnly(int position)
            {
                throw new NotImplementedException();
            }

            public bool IsReadOnly(int position, bool isEdit)
            {
                throw new NotImplementedException();
            }

            public bool IsReadOnly(Span span)
            {
                throw new NotImplementedException();
            }

            public bool IsReadOnly(Span span, bool isEdit)
            {
                throw new NotImplementedException();
            }

            public NormalizedSpanCollection GetReadOnlyExtents(Span span)
            {
                throw new NotImplementedException();
            }

            public IContentType ContentType
            {
                get { throw new NotImplementedException(); }
            }

            public ITextSnapshot CurrentSnapshot
            {
                get { throw new NotImplementedException(); }
            }

            public bool EditInProgress
            {
                get { throw new NotImplementedException(); }
            }

            public event EventHandler<SnapshotSpanEventArgs> ReadOnlyRegionsChanged;
            public event EventHandler<TextContentChangedEventArgs> Changed;
            public event EventHandler<TextContentChangedEventArgs> ChangedLowPriority;
            public event EventHandler<TextContentChangedEventArgs> ChangedHighPriority;
            public event EventHandler<TextContentChangingEventArgs> Changing;
            public event EventHandler PostChanged;
            public event EventHandler<ContentTypeChangedEventArgs> ContentTypeChanged;
        }

        private class FakeVersion : ITextVersion
        {
            public FakeVersion(int versionNumber)
            {
                VersionNumber = versionNumber;
            }

            public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode)
            {
                throw new NotImplementedException();
            }

            public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
            {
                throw new NotImplementedException();
            }

            public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode)
            {
                throw new NotImplementedException();
            }

            public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
            {
                throw new NotImplementedException();
            }

            public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode)
            {
                throw new NotImplementedException();
            }

            public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
            {
                throw new NotImplementedException();
            }

            public ITrackingSpan CreateCustomTrackingSpan(Span span, TrackingFidelityMode trackingFidelity, object customState, CustomTrackToVersion behavior)
            {
                throw new NotImplementedException();
            }

            public ITextVersion Next
            {
                get { throw new NotImplementedException(); }
            }

            public int Length
            {
                get { throw new NotImplementedException(); }
            }

            public INormalizedTextChangeCollection Changes
            {
                get { throw new NotImplementedException(); }
            }

            public ITextBuffer TextBuffer
            {
                get { throw new NotImplementedException(); }
            }

            public int VersionNumber { get; private set; }

            public int ReiteratedVersionNumber
            {
                get { throw new NotImplementedException(); }
            }
        }

        #endregion
    }

}