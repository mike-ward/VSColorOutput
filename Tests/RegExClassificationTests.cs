using System;
using FluentAssertions;
using NUnit.Framework;
using VSColorOutput.Output.ColorClassifier;

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

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void RegExPatternCannotBeSetToNull()
        {
            new RegExClassification {RegExPattern = null};
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void BadRegExExpressionShouldRaiseException()
        {
            new RegExClassification {RegExPattern = @"(\d"};
        }
    }
}