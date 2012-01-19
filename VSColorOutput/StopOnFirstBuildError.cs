// Copyright (c) 2012 Blue Onion Software, All rights reserved
using System;
using EnvDTE;
using EnvDTE80;

#pragma warning disable 649

namespace BlueOnionSoftware
{
    public class StopOnFirstBuildError
    {
        private const string CancelBuildCommand = "Build.Cancel";
        private readonly DTE2 _dte2;
        public bool Enabled { get; set; }

        public StopOnFirstBuildError(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                return;
            }
            _dte2 = serviceProvider.GetService(typeof(DTE)) as DTE2;
            if (_dte2 != null)
            {
                _dte2.Events.BuildEvents.OnBuildProjConfigDone += OnBuildProjectDone;
            }
        }

        private void OnBuildProjectDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            if (Enabled && success == false)
            {
                _dte2.ExecuteCommand(CancelBuildCommand);
            }
        }
    }
}