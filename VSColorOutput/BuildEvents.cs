// Copyright (c) 2012 Blue Onion Software. All rights reserved.

using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using System.Collections.Generic;

#pragma warning disable 649

namespace BlueOnionSoftware
{
    public class BuildEvents
    {
        readonly DTE2 _dte2;
        readonly Events _events;
        readonly EnvDTE.BuildEvents _buildEvents;
        readonly DTEEvents _dteEvents;
        DateTime _buildStartTime;
        readonly List<string> _projectsBuildReport;

        public bool StopOnBuildErrorEnabled { get; set; }
        public bool ShowElapsedBuildTimeEnabled { get; set; }
        public bool ShowBuildReport { get; set; }
        public bool ShowDebugWindowOnDebug { get; set; }

        public BuildEvents(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                return;

            _dte2 = serviceProvider.GetService(typeof(DTE)) as DTE2;
            if (_dte2 != null)
            {
                // These event sources have to be rooted or the GC will collect them.
                // http://social.msdn.microsoft.com/Forums/en-US/vsx/thread/fd2f9108-1df3-4d96-a65d-67a69347ca27
                _events = _dte2.Events;
                _buildEvents = _events.BuildEvents;
                _dteEvents = _events.DTEEvents;

                _buildEvents.OnBuildBegin += OnBuildBegin;
                _buildEvents.OnBuildDone += OnBuildDone;
                _buildEvents.OnBuildProjConfigDone += OnBuildProjectDone;
                _dteEvents.ModeChanged += OnModeChanged;
            }

            _projectsBuildReport = new List<string>();

        }

        private void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            _projectsBuildReport.Clear();
            _buildStartTime = DateTime.Now;
        }

        private void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            OutputWindowPane BuildOutputPane = null;
            foreach (OutputWindowPane pane in _dte2.ToolWindows.OutputWindow.OutputWindowPanes)
            {
                if (pane.Guid == VSConstants.OutputWindowPaneGuid.BuildOutputPane_string)
                {
                    BuildOutputPane = pane;
                    break;
                }
            }

            if (BuildOutputPane == null)
                return;

            if (ShowBuildReport)
            {
                BuildOutputPane.OutputString("\r\nProjects build report:\r\n");
                BuildOutputPane.OutputString("  Status    | Project [Config|platform]\r\n");
                BuildOutputPane.OutputString(" -----------|---------------------------------------------------------------------------------------------------\r\n");

                foreach (string ReportItem in _projectsBuildReport)
                    BuildOutputPane.OutputString(ReportItem + "\r\n");
            }

            if (ShowElapsedBuildTimeEnabled)
            {
                var elapsed = DateTime.Now - _buildStartTime;
                var time = elapsed.ToString(@"hh\:mm\:ss\.ff");
                var text = string.Format("Time Elapsed {0}", time);

                BuildOutputPane.OutputString("\r\n" + text + "\r\n");
            }
        }

        private void OnBuildProjectDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            if (StopOnBuildErrorEnabled && success == false)
            {
                const string cancelBuildCommand = "Build.Cancel";
                _dte2.ExecuteCommand(cancelBuildCommand);
            }

            if (ShowBuildReport)
                _projectsBuildReport.Add("  " + (success ? "Succeeded" : "Failed   ") + " | " + project + " [" + projectConfig + "|" + platform + "]" );
        }

        private void OnModeChanged(vsIDEMode lastMode)
        {
            if (lastMode == vsIDEMode.vsIDEModeDesign && ShowDebugWindowOnDebug)
            {
                _dte2.ToolWindows.OutputWindow.Parent.Activate();
                foreach (OutputWindowPane pane in _dte2.ToolWindows.OutputWindow.OutputWindowPanes)
                {
                    if (pane.Guid == VSConstants.OutputWindowPaneGuid.DebugPane_string)
                    {
                        pane.Activate();
                        break;
                    }
                }
            }
        }
    }
}