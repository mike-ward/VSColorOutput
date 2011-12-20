// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System;

namespace BlueOnionSoftware
{
    public class RegExClassification
    {
        public string RegExPattern { get; set; }
        public ClassificationTypes ClassificationType { get; set; }
        public bool IgnoreCase { get; set; }

        public override string ToString()
        {
            return String.Format("\"{0}\",{1},{2}", RegExPattern ?? "null", ClassificationType, IgnoreCase);
        }
    }
}