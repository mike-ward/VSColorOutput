using System.ComponentModel.Composition;
using FluentAssertions;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using NUnit.Framework;
using VSColorOutput.Output.TimeStamp;

namespace Tests
{
    [TestFixture]
    public class TimeStampMarginProviderTests
    {
        [Test]
        public void GetClassifierAttributes()
        {
            var t = typeof(TimeStampMarginProvider);
            t.Should().BeDecoratedWith<NameAttribute>(a => a.Name == "TimeStampMargin");
            t.Should().BeDecoratedWith<ContentTypeAttribute>(a => a.ContentTypes == "DebugOutput");
            t.Should().BeDecoratedWith<ExportAttribute>(a => a.ContractType == typeof(IWpfTextViewMarginProvider));
            t.Should().BeDecoratedWith<OrderAttribute>(a => a.Before == PredefinedMarginNames.Spacer);
            t.Should().BeDecoratedWith<TextViewRoleAttribute>(a => a.TextViewRoles == PredefinedTextViewRoles.Interactive);
            t.Should().BeDecoratedWith<MarginContainerAttribute>(a => a.MarginContainer == PredefinedMarginNames.LeftSelection);
            t.Should().Implement<IWpfTextViewMarginProvider>();
        }
    }
}