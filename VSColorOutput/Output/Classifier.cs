using System;

// ReSharper disable EmptyGeneralCatchClause
#pragma warning disable 67

namespace VSColorOutput.Output
{
    public struct Classifier
    {
        public string Type { get; set; }
        public Predicate<string> Test { get; set; }
    }
}