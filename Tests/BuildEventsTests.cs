// Copyright (c) 2012 Blue Onion Software, All rights reserved
using System;
using EnvDTE;
using EnvDTE80;
using Moq;
using NUnit.Framework;

// ReSharper disable RedundantArgumentDefaultValue

namespace Tests
{
    [TestFixture]
    public class BuildEventsTests
    {
        [Test]
        public void OnProjectBuildDoneConstructorShouldWireupDteEvents()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockBuildEvents = new Mock<BuildEvents>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(DTE))).Returns(mockDte2.Object);
            mockDte2.SetupGet(d => d.Events).Returns(mockEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            new BlueOnionSoftware.BuildEvents(mockServiceProvider.Object);

            mockDte2.VerifyAll();
            mockEvents.VerifyAll();
            mockBuildEvents.VerifyAll();
            mockServiceProvider.VerifyAll();
        }

        public interface IDispBuildEvents
        {
            event _dispBuildEvents_OnBuildBeginEventHandler BuildBeginEvent;
            event _dispBuildEvents_OnBuildDoneEventHandler BuildEndEvent;
            event _dispBuildEvents_OnBuildProjConfigDoneEventHandler BuildProjConfigEvent;
        }

        [Test]
        public void OnBuildProjectDoneCancelsBuildOnErrorWhenEnabled()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockBuildEvents = new Mock<BuildEvents>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(DTE))).Returns(mockDte2.Object);
            mockDte2.SetupGet(d => d.Events).Returns(mockEvents.Object);
            mockDte2.Setup(d => d.ExecuteCommand("Build.Cancel", ""));
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new BlueOnionSoftware.BuildEvents(mockServiceProvider.Object);
            buildEvents.StopOnBuildErrorEnabled = true;
            mockBuildEvents.Raise(be => be.OnBuildProjConfigDone += null, "", "", "", "", false);

            mockDte2.VerifyAll();
            mockEvents.VerifyAll();
            mockBuildEvents.VerifyAll();
            mockServiceProvider.VerifyAll();
        }

        [Test]
        public void OnBuildDoneShowsElapsedTimeWhenEnabled()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockBuildEvents = new Mock<BuildEvents>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockToolWindows = new Mock<ToolWindows>();
            var mockOutputWindow = new Mock<OutputWindow>();
            var mockOutputWindowPanes = new Mock<OutputWindowPanes>();
            var mockOutputWindowPane = new Mock<OutputWindowPane>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(DTE))).Returns(mockDte2.Object);
            mockDte2.SetupGet(d => d.Events).Returns(mockEvents.Object);
            mockDte2.SetupGet(d => d.ToolWindows).Returns(mockToolWindows.Object);
            mockToolWindows.SetupGet(t => t.OutputWindow).Returns(mockOutputWindow.Object);
            mockOutputWindow.SetupGet(o => o.OutputWindowPanes).Returns(mockOutputWindowPanes.Object);
            mockOutputWindowPane.SetupGet(op => op.Guid).Returns("{1BD8A850-02D1-11D1-BEE7-00A0C913D1F8}");
            mockOutputWindowPane.Setup(op => op.OutputString(It.IsAny<string>()));
            var panes = new [] {mockOutputWindowPane.Object};
            mockOutputWindowPanes.Setup(op => op.GetEnumerator()).Returns(panes.GetEnumerator());
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new BlueOnionSoftware.BuildEvents(mockServiceProvider.Object);
            buildEvents.ShowElapsedBuildTimeEnabled = true;
            mockBuildEvents.Raise(be => be.OnBuildBegin += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);
            mockBuildEvents.Raise(be => be.OnBuildDone += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);

            mockDte2.VerifyAll();
            mockEvents.VerifyAll();
            mockBuildEvents.VerifyAll();
            mockServiceProvider.VerifyAll();
            mockToolWindows.VerifyAll();
            mockOutputWindow.VerifyAll();
            mockOutputWindowPane.VerifyAll();
            mockOutputWindowPanes.VerifyAll();
        }
    }
}