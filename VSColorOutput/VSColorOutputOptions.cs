// Copyright (c) 2011 Blue Onion Software, All rights reserved
using Microsoft.VisualStudio.Shell;

namespace BlueOnionSoftware
{
    public class VsColorOutputOptions : DialogPage
    {
        public PatternClassificationType[] Patterns { get; set; }
    }

    public class PatternClassificationType
    {
        public string Pattern { get; set; }
        public string ClassificationType { get; set; }
    }
}