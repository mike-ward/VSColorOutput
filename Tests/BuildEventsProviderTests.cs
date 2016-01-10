using System.ComponentModel.Composition;
using FluentAssertions;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NUnit.Framework;
using VSColorOutput.Output.BuildEvents;

namespace Tests
{
    [TestFixture]
    public class BuildEventsProviderTests
    {
        [Test]
        public void GetClassifierAttributes()
        {
            typeof(BuildEventsProvider).Should().BeDecoratedWith<ContentTypeAttribute>(a => a.ContentTypes.Equals("output"));
            typeof(BuildEventsProvider).Should().BeDecoratedWith<ExportAttribute>(a => a.ContractType == typeof(IClassifierProvider));
            typeof(BuildEventsProvider).Should().Implement<IClassifierProvider>();
        }
    }
}