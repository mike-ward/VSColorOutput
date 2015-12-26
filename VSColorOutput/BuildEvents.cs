using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

#pragma warning disable 649

namespace BlueOnionSoftware
{
    public class BuildEvents
    {
        private readonly DTE2 _dte2;
        private readonly Events _events;
        private readonly EnvDTE.BuildEvents _buildEvents;
        private readonly DTEEvents _dteEvents;
        private DateTime _buildStartTime;
        private readonly List<string> _projectsBuildReport;
        public bool StopOnBuildErrorEnabled { private get; set; }
        public bool ShowElapsedBuildTimeEnabled { private get; set; }
        public bool ShowBuildReport { private get; set; }
        public bool ShowDebugWindowOnDebug { private get; set; }

        public BuildEvents(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                return;
            }
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

            Settings.SettingsUpdated += (sender, args) => LoadSettings();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = Settings.Load();
            StopOnBuildErrorEnabled = settings.EnableStopOnBuildError;
            ShowElapsedBuildTimeEnabled = settings.ShowElapsedBuildTime;
            ShowBuildReport = settings.ShowBuildReport;
            ShowDebugWindowOnDebug = settings.ShowDebugWindowOnDebug;
        }

        private void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            _projectsBuildReport.Clear();
            _buildStartTime = DateTime.Now;
        }

        private void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            OutputWindowPane buildOutputPane = null;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (OutputWindowPane pane in _dte2.ToolWindows.OutputWindow.OutputWindowPanes)
            {
                if (pane.Guid == VSConstants.OutputWindowPaneGuid.BuildOutputPane_string)
                {
                    buildOutputPane = pane;
                    break;
                }
            }

            if (buildOutputPane == null)
            {
                return;
            }

            if (ShowBuildReport)
            {
                buildOutputPane.OutputString("\r\nProjects build report:\r\n");
                buildOutputPane.OutputString("  Status    | Project [Config|platform]\r\n");
                buildOutputPane.OutputString(" -----------|---------------------------------------------------------------------------------------------------\r\n");
                foreach (var reportItem in _projectsBuildReport)
                {
                    buildOutputPane.OutputString(reportItem + "\r\n");
                }
            }

            if (ShowElapsedBuildTimeEnabled)
            {
                var elapsed = DateTime.Now - _buildStartTime;
                var time = elapsed.ToString(@"hh\:mm\:ss\.ff");
                var text = $"Time Elapsed {time}";
                buildOutputPane.OutputString("\r\n" + text + "\r\n");
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
            {
                _projectsBuildReport.Add("  " + (success ? "Succeeded" : "Failed   ") + " | " + project + " [" + projectConfig + "|" + platform + "]");
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