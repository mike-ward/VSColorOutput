// Copyright (c) 2012 Blue Onion Software, All rights reserved
using System;
using BlueOnionSoftware;
using EnvDTE;
using EnvDTE80;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class StopBuildOnFirstErrorTests
    {
        [Test]
        public void OnProjectBuildDoneNotEnabledAndFaileShouldCancelBuild()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockBuildEvents = new Mock<BuildEvents>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(DTE))).Returns(mockDte2.Object);
            mockDte2.SetupGet(d => d.Events).Returns(mockEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            new StopOnFirstBuildError(mockServiceProvider.Object);

            mockDte2.VerifyAll();
            mockEvents.VerifyAll();
            mockBuildEvents.VerifyAll();
        }
    }
}