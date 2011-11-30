// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System.ComponentModel.Composition;
using BlueOnionSoftware;
using FluentAssertions;
using Microsoft.VisualStudio.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class OutputClassifierProviderTests
    {
        [Test]
        public void GetClassifierAttributes()
        {
            typeof(OutputClassifierProvider).Should().BeDecoratedWith<ContentTypeAttribute>();
            typeof(OutputClassifierProvider).Should().BeDecoratedWith<ExportAttribute>();
        }

        [Test]
        public void GetClassifierReturnsSameInstance()
        {
            var ocp = new OutputClassifierProvider();
            var classifier = ocp.GetClassifier(null);
            classifier.Should().NotBeNull();
            classifier.Should().BeSameAs(ocp.GetClassifier(null));
        }
    }
}