using System.ComponentModel.Composition;
using FluentAssertions;
using Microsoft.VisualStudio.Utilities;
using NUnit.Framework;
using VSColorOutput;

namespace Tests
{
    [TestFixture]
    public class FindResultsClassifierProviderTests
    {
        [Test]
        public void GetClassifierAttributes()
        {
            typeof(FindResultsClassifierProvider).Should().BeDecoratedWith<ContentTypeAttribute>();
            typeof(FindResultsClassifierProvider).Should().BeDecoratedWith<ExportAttribute>();
        }

        [Test]
        public void GetClassifierReturnsSameInstance()
        {
            var provider = new FindResultsClassifierProvider();
            var classifier = provider.GetClassifier(null);
            classifier.Should().NotBeNull();
            classifier.Should().BeSameAs(provider.GetClassifier(null));
        }
    }
}