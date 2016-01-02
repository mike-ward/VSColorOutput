using System.ComponentModel.Composition;
using FluentAssertions;
using Microsoft.VisualStudio.Utilities;
using NUnit.Framework;
using VSColorOutput.Output.ColorClassifier;

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