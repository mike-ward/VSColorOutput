using System.ComponentModel.Composition;
using FluentAssertions;
using Microsoft.VisualStudio.Text.Classification;
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
            typeof(OutputClassifierProvider).Should().BeDecoratedWith<ContentTypeAttribute>(a => a.ContentTypes.Equals("output"));
            typeof(OutputClassifierProvider).Should().BeDecoratedWith<ExportAttribute>(a => a.ContractType == typeof(IClassifierProvider));
        }
    }
}