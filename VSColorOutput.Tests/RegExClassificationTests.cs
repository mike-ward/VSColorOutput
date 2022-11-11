using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSColorOutput.Output.ColorClassifier;

// ReSharper disable ObjectCreationAsStatement

namespace Tests
{
    [TestClass]
    public class RegExClassificationTests
    {
        [TestMethod]
        public void ToStringFormat()
        {
            var rc = new RegExClassification
            {
                RegExPattern       = "/d",
                ClassificationType = ClassificationTypes.BuildText,
                IgnoreCase         = true
            };
            rc.ToString().Should().Be("\"/d\",BuildText,True");
        }

        [TestMethod]
        public void RegExPatternCannotBeSetToNull()
        {
            Action act = () => new RegExClassification { RegExPattern = null };
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void BadRegExExpressionShouldRaiseException()
        {
            Action act = () => new RegExClassification { RegExPattern = @"(\d" };
            act.Should().Throw<ArgumentException>();
        }
    }
}