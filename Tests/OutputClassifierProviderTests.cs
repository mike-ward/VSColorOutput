// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System.ComponentModel.Composition;
using BlueOnionSoftware;
using FluentAssertions;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Moq;
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
        public void GetClassifierRegistersForNotificationReturnsSameInstance()
        {
            var mockTextManager2 = new Mock<IVsTextManager2>();
            var mockConnectionPointContainer = mockTextManager2.As<IConnectionPointContainer>();
            TextManagerEvents.Override = mockTextManager2.Object;

            try
            {
                var eventGuid = typeof(IVsTextManagerEvents).GUID;
                var mockTextManagerEventsConnection = new Mock<IConnectionPoint>();
                var textManagerEventsConnection = mockTextManagerEventsConnection.Object;
                mockConnectionPointContainer
                    .Setup(cpc => cpc.FindConnectionPoint(ref eventGuid, out textManagerEventsConnection));

                uint cookie;
                mockTextManagerEventsConnection
                    .Setup(tm => tm.Advise(It.IsAny<TextManagerEvents>(), out cookie));

                var ocp = new OutputClassifierProvider();
                var classifier = ocp.GetClassifier(null);
                classifier.Should().NotBeNull();
                classifier.Should().BeSameAs(ocp.GetClassifier(null));

                mockTextManager2.VerifyAll();
                mockConnectionPointContainer.VerifyAll();
                mockTextManagerEventsConnection.VerifyAll();
            }
            finally
            {
                TextManagerEvents.Override = null;
            }
        }
    }
}