using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
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
            var mockDteEvents = new Mock<DTEEvents>();
            var mockBuildEvents = new Mock<BuildEvents>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(DTE))).Returns(mockDte2.Object);
            mockDte2.SetupGet(d => d.Events).Returns(mockEvents.Object);
            mockEvents.SetupGet(e => e.DTEEvents).Returns(mockDteEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            new VSColorOutput.Output.BuildEvents.BuildEvents(mockServiceProvider.Object);

            mockDte2.VerifyAll();
            mockEvents.VerifyAll();
            mockDteEvents.VerifyAll();
            mockBuildEvents.VerifyAll();
            mockServiceProvider.VerifyAll();
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

        [Test]
        public void OnBuildProjectDoneCancelsBuildOnErrorWhenEnabled()
        {
            var mockDte2 = new Mock<DTE2>();
            var mockEvents = new Mock<Events>();
            var mockDteEvents = new Mock<DTEEvents>();
            var mockBuildEvents = new Mock<BuildEvents>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(DTE))).Returns(mockDte2.Object);
            mockDte2.SetupGet(d => d.Events).Returns(mockEvents.Object);
            mockDte2.Setup(d => d.ExecuteCommand("Build.Cancel", ""));
            mockEvents.SetupGet(e => e.DTEEvents).Returns(mockDteEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new VSColorOutput.Output.BuildEvents.BuildEvents(mockServiceProvider.Object);
            buildEvents.StopOnBuildErrorEnabled = true;
            mockBuildEvents.Raise(be => be.OnBuildProjConfigDone += null, "", "", "", "", false);

            mockDte2.VerifyAll();
            mockEvents.VerifyAll();
            mockDteEvents.VerifyAll();
            mockBuildEvents.VerifyAll();
            mockServiceProvider.VerifyAll();
        }

        [Test]
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
            mockOutputWindowPane.Setup(op => op.OutputString(It.IsRegex("Time Elapsed .*")));
            var panes = new[] { mockOutputWindowPane.Object };
            mockOutputWindowPanes.Setup(op => op.GetEnumerator()).Returns(panes.GetEnumerator());
            mockEvents.SetupGet(e => e.DTEEvents).Returns(mockDteEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new VSColorOutput.Output.BuildEvents.BuildEvents(mockServiceProvider.Object);
            buildEvents.ShowElapsedBuildTimeEnabled = true;
            mockBuildEvents.Raise(be => be.OnBuildBegin += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);
            mockBuildEvents.Raise(be => be.OnBuildDone += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);

            mockDte2.VerifyAll();
            mockEvents.VerifyAll();
            mockDteEvents.VerifyAll();
            mockBuildEvents.VerifyAll();
            mockServiceProvider.VerifyAll();
            mockToolWindows.VerifyAll();
            mockOutputWindow.VerifyAll();
            mockOutputWindowPane.VerifyAll();
            mockOutputWindowPanes.VerifyAll();
        }

        [Test]
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
            mockOutputWindowPane.Setup(op => op.OutputString(It.Is<string>(s => s =="\r\nProjects build report:\r\n")));
            mockOutputWindowPane.Setup(op => op.OutputString(It.Is<string>(s => s == "  Succeeded | test.proj [Debug|Any CPU]\r\n")));
            mockOutputWindowPane.Setup(op => op.OutputString(It.Is<string>(s => s == "  Failed    | test2.proj [Release|Any CPU]\r\n")));
            var panes = new[] { mockOutputWindowPane.Object };
            mockOutputWindowPanes.Setup(op => op.GetEnumerator()).Returns(panes.GetEnumerator());
            mockEvents.SetupGet(e => e.DTEEvents).Returns(mockDteEvents.Object);
            mockEvents.SetupGet(e => e.BuildEvents).Returns(() => mockBuildEvents.Object);

            var buildEvents = new VSColorOutput.Output.BuildEvents.BuildEvents(mockServiceProvider.Object);
            buildEvents.ShowBuildReport = true;
            mockBuildEvents.Raise(be => be.OnBuildBegin += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);
            mockBuildEvents.Raise(be => be.OnBuildProjConfigDone += null, "test.proj", "Debug", "Any CPU", "", true);
            mockBuildEvents.Raise(be => be.OnBuildProjConfigDone += null, "test2.proj", "Release", "Any CPU", "", false);
            mockBuildEvents.Raise(be => be.OnBuildDone += null, vsBuildScope.vsBuildScopeSolution, vsBuildAction.vsBuildActionBuild);

            mockDte2.VerifyAll();
            mockEvents.VerifyAll();
            mockDteEvents.VerifyAll();
            mockBuildEvents.VerifyAll();
            mockServiceProvider.VerifyAll();
            mockToolWindows.VerifyAll();
            mockOutputWindow.VerifyAll();
            mockOutputWindowPane.VerifyAll();
            mockOutputWindowPanes.VerifyAll();
        }
        [Test]
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

            var buildEvents = new VSColorOutput.Output.BuildEvents.BuildEvents(mockServiceProvider.Object);
            buildEvents.ShowDebugWindowOnDebug = true;
            mockDteEvents.Raise(de => de.ModeChanged += null, vsIDEMode.vsIDEModeDesign);

            mockDte2.VerifyAll();
            mockEvents.VerifyAll();
            mockDteEvents.VerifyAll();
            mockBuildEvents.VerifyAll();
            mockServiceProvider.VerifyAll();
            mockToolWindows.VerifyAll();
            mockOutputWindow.VerifyAll();
            mockOutputWindowPane.VerifyAll();
            mockOutputWindowPanes.VerifyAll();
            mockWindow.VerifyAll();
        }
    }
}