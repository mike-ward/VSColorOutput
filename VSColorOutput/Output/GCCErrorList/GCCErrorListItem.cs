using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace VSColorOutput.Output.GCCErrorList
{
    public class GCCErrorListItem
    {
        public int ProjectNumber { get; private set; }

        public string Filename { get; private set; }

        public int Line { get; private set; }

        public int Column { get; private set; }

        public string Text { get; private set; }

        public GCCErrorType ErrorType { get; private set; }

        internal static GCCErrorListItem Parse(string text)
        {
            // Will recognize only form of 1>main.c:1:2
            var fullMatches = Regex.Matches(text, @"^(\d+)>(.*?):(\d*):(\d*).*:(.*)");
            if (fullMatches.Count > 0)
            {
                var groups = fullMatches[0].Groups;
                return new GCCErrorListItem
                {
                    ProjectNumber = int.Parse(groups[1].Value),
                    Filename = groups[2].Value,
                    Line = int.Parse(groups[3].Value),
                    Column = int.Parse(groups[4].Value),
                    Text = groups[5].Value,
                    ErrorType = GCCErrorType.Full
                };
            }

            var gccMatches = Regex.Matches(text, @"^(\d+)>gcc:.*?:(.*)");
            if (gccMatches.Count > 0)
            {
                var groups = gccMatches[0].Groups;
                return new GCCErrorListItem
                {
                    ProjectNumber = int.Parse(groups[1].Value),
                    Filename = "gcc",
                    Text = groups[2].Value,
                    ErrorType = GCCErrorType.GCCOnly
                };
            }

            // Nothing parsed. Probably not a GCC error
            return null;
        }

        protected bool Equals(GCCErrorListItem other)
        {
            return ProjectNumber == other.ProjectNumber
                && string.Equals(Filename, other.Filename)
                && Line == other.Line
                && Column == other.Column
                && string.Equals(Text, other.Text)
                && ErrorType == other.ErrorType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GCCErrorListItem)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ProjectNumber;
                hashCode = (hashCode * 397) ^ (Filename != null
                    ? Filename.GetHashCode()
                    : 0);
                hashCode = (hashCode * 397) ^ Line;
                hashCode = (hashCode * 397) ^ Column;
                hashCode = (hashCode * 397) ^ (Text != null
                    ? Text.GetHashCode()
                    : 0);
                hashCode = (hashCode * 397) ^ ErrorType.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(GCCErrorListItem left, GCCErrorListItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GCCErrorListItem left, GCCErrorListItem right)
        {
            return !Equals(left, right);
        }
    }
}