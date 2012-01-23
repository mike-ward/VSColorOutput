// Copyright (c) 2012 Blue Onion Software. All rights reserved.

using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;

#pragma warning disable 649

namespace BlueOnionSoftware
{
    public class BuildEvents
    {
        private readonly DTE2 _dte2;
        private DateTime _buildStartTime;
        public bool StopOnBuildErrorEnabled { get; set; }
        public bool ShowElapsedBuildTimeEnabled { get; set; }
        public bool ShowDebugWindowOnDebug { get; set; }

        public BuildEvents(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                return;
            }
            _dte2 = serviceProvider.GetService(typeof(DTE)) as DTE2;
            if (_dte2 != null)
            {
                _dte2.Events.BuildEvents.OnBuildBegin += OnBuildBegin;
                _dte2.Events.BuildEvents.OnBuildDone += OnBuildDone;
                _dte2.Events.BuildEvents.OnBuildProjConfigDone += OnBuildProjectDone;
                _dte2.Events.DTEEvents.ModeChanged += OnModeChanged;
            }
        }

        private void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            if (scope != vsBuildScope.vsBuildScopeProject)
            {
                _buildStartTime = DateTime.Now;
            }
        }

        private void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            if (scope != vsBuildScope.vsBuildScopeProject && ShowElapsedBuildTimeEnabled)
            {
                var elapsed = DateTime.Now - _buildStartTime;
                foreach (OutputWindowPane pane in _dte2.ToolWindows.OutputWindow.OutputWindowPanes)
                {
                    if (pane.Guid == VSConstants.OutputWindowPaneGuid.BuildOutputPane_string)
                    {
                        var time = elapsed.ToString(@"hh\:mm\:ss\.ff");
                        var text = string.Format("Time Elapsed {0}", time);
                        pane.OutputString("\r\n" + text + "\r\n");
                        break;
                    }
                }
            }
        }

        private void OnBuildProjectDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            if (StopOnBuildErrorEnabled && success == false)
            {
                const string cancelBuildCommand = "Build.Cancel";
                _dte2.ExecuteCommand(cancelBuildCommand);
            }
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