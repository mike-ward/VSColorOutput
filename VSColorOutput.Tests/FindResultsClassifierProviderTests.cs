using System.ComponentModel.Composition;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Utilities;
using VSColorOutput.FindResults;

namespace Tests
{
    [TestClass]
    public class FindResultsClassifierProviderTests
    {
        [TestMethod]
        public void GetClassifierAttributes()
        {
            typeof(FindResultsClassifierProvider).Should().BeDecoratedWith<ContentTypeAttribute>();
            typeof(FindResultsClassifierProvider).Should().BeDecoratedWith<ExportAttribute>();
        }

        [TestMethod]
        public void GetClassifierReturnsSameInstance()
        {
            var provider   = new FindResultsClassifierProvider();
            var classifier = provider.GetClassifier(null);
            classifier.Should().NotBeNull();
            classifier.Should().BeSameAs(provider.GetClassifier(null));
        }
    }
}