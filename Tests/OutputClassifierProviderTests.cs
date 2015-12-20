// Copyright (c) 2012 Blue Onion Software, All rights reserved

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
            typeof (OutputClassifierProvider).Should().BeDecoratedWith<ContentTypeAttribute>();
            typeof (OutputClassifierProvider).Should().BeDecoratedWith<ExportAttribute>();
        }
    }
}