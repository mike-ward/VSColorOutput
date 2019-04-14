using System.ComponentModel.Composition;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using VSColorOutput.Output.ColorClassifier;

namespace Tests
{
    [TestClass]
    public class OutputClassifierProviderTests
    {
        [TestMethod]
        public void GetClassifierAttributes()
        {
            typeof(OutputClassifierProvider).Should().BeDecoratedWith<ContentTypeAttribute>(a => a.ContentTypes.Equals("output"));
            typeof(OutputClassifierProvider).Should().BeDecoratedWith<ExportAttribute>(a => a.ContractType == typeof(IClassifierProvider));
        }
    }
}