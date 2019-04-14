using System.ComponentModel.Composition;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using VSColorOutput.Output.BuildEvents;

namespace Tests
{
    [TestClass]
    public class BuildEventsProviderTests
    {
        [TestMethod]
        public void GetClassifierAttributes()
        {
            typeof(BuildEventsProvider).Should().BeDecoratedWith<ContentTypeAttribute>(a => a.ContentTypes.Equals("output"));
            typeof(BuildEventsProvider).Should().BeDecoratedWith<ExportAttribute>(a => a.ContractType == typeof(IClassifierProvider));
            typeof(BuildEventsProvider).Should().Implement<IClassifierProvider>();
        }
    }
}