using System;
using FluentAssertions;
using NUnit.Framework;
using VSColorOutput.Output.ColorClassifier;
// ReSharper disable ObjectCreationAsStatement

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
        public void RegExPatternCannotBeSetToNull()
        {
            Action act = () => new RegExClassification {RegExPattern = null};
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void BadRegExExpressionShouldRaiseException()
        {
            Action act = () => new RegExClassification {RegExPattern = @"(\d"};
            act.ShouldThrow<ArgumentException>();
        }
    }
}