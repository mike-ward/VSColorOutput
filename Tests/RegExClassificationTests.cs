// Copyright (c) 2011 Blue Onion Software, All rights reserved
using BlueOnionSoftware;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class RegExClassificationTests
    {
        [Test]
        public void ToStringFormat()
        {
            var rc = new RegExClassification
            {
                RegExPattern = "/d",
                ClassificationType = ClassificationTypes.BuildText,
                IgnoreCase = true
            };
            rc.ToString().Should().Be("\"/d\",BuildText,True");
        }

        [Test]
        public void ToStringFormatWithNull()
        {
            var rc = new RegExClassification
            {
                RegExPattern = null,
                ClassificationType = ClassificationTypes.LogCustom4,
                IgnoreCase = false
            };
            rc.ToString().Should().Be("\"null\",LogCustom4,False");
        }
    }
}