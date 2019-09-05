using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using VSColorOutput.Output.TimeStamp;
using VSColorOutput.State;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

#pragma warning disable 649

namespace VSColorOutput.Output.BuildEvents
{
    public class BuildEvents
    {
        private DTE2 _dte2;
        private Events _events;
        private EnvDTE.BuildEvents _buildEvents;
        private int _initialized;
        private DTEEvents _dteEvents;
        private DateTime _buildStartTime;
        private List<string> _projectsBuildReport;

        public bool StopOnBuildErrorEnabled { get; set; }
        public bool ShowElapsedBuildTimeEnabled { get; set; }
        public bool ShowBuildReport { get; set; }
        public bool ShowDebugWindowOnDebug { get; set; }
        public bool ShowTimeStamps { get; set; }
        public DateTime DebugStartTime { get; private set; }
        public bool ShowDonation { get; set; }

        public void Initialize(IServiceProvider serviceProvider)
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 1) return;

#pragma warning disable VSSDK006 // Check services exist
            _dte2 = serviceProvider.GetService(typeof(DTE)) as DTE2;
#pragma warning restore VSSDK006 // Check services exist
            if (_dte2 != null)
            {
                // These event sources have to be rooted or the GC will collect them.
                // https://social.msdn.microsoft.com/Forums/en-US/vsx/thread/fd2f9108-1df3-4d96-a65d-67a69347ca27
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
            ShowTimeStamps = settings.ShowTimeStamps;
            ShowDonation = !settings.SuppressDonation;
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
                var now = DateTime.Now;
                var elapsed = now - _buildStartTime;
                var time = elapsed.ToString(@"hh\:mm\:ss\.fff");
                var buildTime = now.ToString("yyyy-MM-dd hh:mm:ss tt", new CultureInfo("en-US"));
                var text = $"Time Elapsed {time}";
                var text2 = $"Build ended at {buildTime}";
                buildOutputPane.OutputString("\r\n" + text + "\r\n");
                buildOutputPane.OutputString(text2 + "\r\n");
            }

            if (ShowDonation)
            {
                buildOutputPane.OutputString("\r\n++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                buildOutputPane.OutputString("\r\n+++                 Please consider donating to VSColorOutput                    +++");
                buildOutputPane.OutputString("\r\n+++                       https://mike-ward.net/donate/                          +++");
                buildOutputPane.OutputString("\r\n+++            (this message can be turned off in the settings panel)            +++");
                buildOutputPane.OutputString("\r\n++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\r\n");
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
            if (lastMode == vsIDEMode.vsIDEModeDesign)
            {
                DebugStartTime = DateTime.Now;

                if (ShowDebugWindowOnDebug || (ShowTimeStamps && !TimeStampMarginProvider.Initialized))
                {
                    ActivateDebugOutputWindow();
                }
            }
        }

        private void ActivateDebugOutputWindow()
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
