using System;
using EnvDTE;
using EnvDTE80;
using FluentAssertions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

// ReSharper disable RedundantArgumentDefaultValue

namespace Tests
{
    [TestClass]
    public class BuildEventsTests
    {
        [TestMethod]
        public void OnProjectBuildDoneConstructorShouldWireupDteEvents()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockDteEvents = new Mock<DTEEvents>();
            var mockBuildEvents = new Mock<BuildEvents>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(DTE))).Returns(mockDte2.Object);
            mockDte2.SetupGet(d => d.Events).Returns(mockEvents.Object);
            mockEvents.SetupGet(e => e.DTEEvents).Returns(mockDteEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new VSColorOutput.Output.BuildEvents.BuildEvents();
            buildEvents.Initialize(mockServiceProvider.Object);

            Mock.VerifyAll();
        }

        public interface IDispBuildEvents
        {
            event _dispBuildEvents_OnBuildBeginEventHandler BuildBeginEvent;

            event _dispBuildEvents_OnBuildDoneEventHandler BuildEndEvent;

            event _dispBuildEvents_OnBuildProjConfigDoneEventHandler BuildProjConfigEvent;
        }

        public interface IDispDteEvents
        {
            event _dispDTEEvents_ModeChangedEventHandler ModeChanged;
        }

        [TestMethod]
        public void OnBuildProjectDoneCancelsBuildOnErrorWhenEnabled()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockDteEvents = new Mock<DTEEvents>();
            var mockBuildEvents = new Mock<BuildEvents>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(DTE))).Returns(mockDte2.Object);
            mockDte2.SetupGet(d => d.Events).Returns(mockEvents.Object);
            mockEvents.SetupGet(e => e.DTEEvents).Returns(mockDteEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new VSColorOutput.Output.BuildEvents.BuildEvents();
            buildEvents.Initialize(mockServiceProvider.Object);
            buildEvents.StopOnBuildErrorEnabled = true;

            Action act = () => mockBuildEvents.Raise(be => be.OnBuildProjConfigDone += null, "", "", "", "", false);
            act.Should().Throw<NullReferenceException>();

            Mock.VerifyAll();
        }

        [TestMethod]
        public void OnBuildDoneShowsElapsedTimeWhenEnabled()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockDteEvents = new Mock<DTEEvents>();
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
            mockOutputWindowPane.SetupGet(op => op.Guid).Returns(VSConstants.OutputWindowPaneGuid.BuildOutputPane_string);
            mockOutputWindowPane.Setup(op => op.OutputString(It.IsRegex("Build time .*")));
            var panes = new[] { mockOutputWindowPane.Object };
            mockOutputWindowPanes.Setup(op => op.GetEnumerator()).Returns(panes.GetEnumerator());
            mockEvents.SetupGet(e => e.DTEEvents).Returns(mockDteEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new VSColorOutput.Output.BuildEvents.BuildEvents();
            buildEvents.Initialize(mockServiceProvider.Object);
            buildEvents.ShowElapsedBuildTimeEnabled = true;
            mockBuildEvents.Raise(be => be.OnBuildBegin += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);
            mockBuildEvents.Raise(be => be.OnBuildDone += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);

            Mock.VerifyAll();
        }

        [TestMethod]
        public void OnBuildDoneShowsProjectsBuildReportWhenEnabled()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockDteEvents = new Mock<DTEEvents>();
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
            mockOutputWindowPane.SetupGet(op => op.Guid).Returns(VSConstants.OutputWindowPaneGuid.BuildOutputPane_string);
            mockOutputWindowPane.Setup(op => op.OutputString(It.Is<string>(s => s == $"{Environment.NewLine}Projects build report:{Environment.NewLine}")));
            mockOutputWindowPane.Setup(op => op.OutputString(It.Is<string>(s => s == $"  Succeeded | test.proj [Debug|Any CPU]{Environment.NewLine}")));
            mockOutputWindowPane.Setup(op => op.OutputString(It.Is<string>(s => s == $"  Failed    | test2.proj [Release|Any CPU]{Environment.NewLine}")));
            var panes = new[] { mockOutputWindowPane.Object };
            mockOutputWindowPanes.Setup(op => op.GetEnumerator()).Returns(panes.GetEnumerator());
            mockEvents.SetupGet(e => e.DTEEvents).Returns(mockDteEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new VSColorOutput.Output.BuildEvents.BuildEvents();
            buildEvents.Initialize(mockServiceProvider.Object);
            buildEvents.ShowBuildReport = true;
            mockBuildEvents.Raise(be => be.OnBuildBegin += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);
            mockBuildEvents.Raise(be => be.OnBuildProjConfigDone += null, "test.proj", "Debug", "Any CPU", "", true);
            mockBuildEvents.Raise(be => be.OnBuildProjConfigDone += null, "test2.proj", "Release", "Any CPU", "", false);
            mockBuildEvents.Raise(be => be.OnBuildDone += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);

            Mock.VerifyAll();
        }

        [TestMethod]
        public void DteEventsModeChangeActivatesDebugOutputWindowWhenEnabled()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockDteEvents = new Mock<DTEEvents>();
            var mockBuildEvents = new Mock<BuildEvents>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockToolWindows = new Mock<ToolWindows>();
            var mockOutputWindow = new Mock<OutputWindow>();
            var mockOutputWindowPanes = new Mock<OutputWindowPanes>();
            var mockOutputWindowPane = new Mock<OutputWindowPane>();
            var mockWindow = new Mock<Window>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(DTE))).Returns(mockDte2.Object);
            mockDte2.SetupGet(d => d.Events).Returns(mockEvents.Object);
            mockDte2.SetupGet(d => d.ToolWindows).Returns(mockToolWindows.Object);
            mockToolWindows.SetupGet(t => t.OutputWindow).Returns(mockOutputWindow.Object);
            mockOutputWindow.SetupGet(o => o.OutputWindowPanes).Returns(mockOutputWindowPanes.Object);
            mockOutputWindow.SetupGet(o => o.Parent).Returns(mockWindow.Object);
            mockWindow.Setup(w => w.Activate());
            mockOutputWindowPane.SetupGet(op => op.Guid).Returns(VSConstants.OutputWindowPaneGuid.DebugPane_string);
            mockOutputWindowPane.Setup(op => op.Activate());
            var panes = new[] { mockOutputWindowPane.Object };
            mockOutputWindowPanes.Setup(op => op.GetEnumerator()).Returns(panes.GetEnumerator());
            mockEvents.SetupGet(e => e.DTEEvents).Returns(mockDteEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new VSColorOutput.Output.BuildEvents.BuildEvents();
            buildEvents.Initialize(mockServiceProvider.Object);
            buildEvents.ShowDebugWindowOnDebug = true;
            mockDteEvents.Raise(de => de.ModeChanged += null, vsIDEMode.vsIDEModeDesign);

            Mock.VerifyAll();
        }
    }
}