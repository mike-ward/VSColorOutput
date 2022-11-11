using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text.Classification;

namespace Tests
{
    public class FakeClassificationType : IClassificationType
    {
        public FakeClassificationType(string classification) => Classification = classification;

        public bool IsOfType(string type)
        {
            return false;
        }

        public string Classification { get; }

        public IEnumerable<IClassificationType> BaseTypes => Enumerable.Empty<IClassificationType>();
    }
}