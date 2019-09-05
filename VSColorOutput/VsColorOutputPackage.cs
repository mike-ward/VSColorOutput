using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using VSColorOutput.State;
using Task = System.Threading.Tasks.Task;

namespace VSColorOutput
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(VSColorOutputPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(VsColorOutputOptionsDialog), VsColorOutputOptionsDialog.Category, VsColorOutputOptionsDialog.SubCategory, 1000, 1001, true)]
    [ProvideProfile(typeof(VsColorOutputOptionsDialog), VsColorOutputOptionsDialog.Category, VsColorOutputOptionsDialog.SubCategory, 1000, 1001, true)]
    [InstalledProductRegistration("VSColorOutput", "Color output for build and debug windows - https://mike-ward.net/vscoloroutput", "2.6.6")]
    public sealed class VSColorOutputPackage : AsyncPackage
    {
        public const string PackageGuidString = "CD56B219-38CB-482A-9B2D-7582DF4AAF1E";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }
    }
}