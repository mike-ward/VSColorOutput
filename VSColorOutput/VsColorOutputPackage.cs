using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSColorOutput.State;

namespace VSColorOutput
{
    [Guid("CD56B219-38CB-482A-9B2D-7582DF4AAF1E")]
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\14.0")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideOptionPage(typeof(VsColorOutputOptionsDialog), VsColorOutputOptionsDialog.Category, VsColorOutputOptionsDialog.SubCategory, 1000, 1001, true)]
    [ProvideProfile(typeof(VsColorOutputOptionsDialog), VsColorOutputOptionsDialog.Category, VsColorOutputOptionsDialog.SubCategory, 1000, 1001, true)]
    [InstalledProductRegistration("VSColorOutput", "Color output for build and debug windows - http://mike-ward.net/vscoloroutput", "2.6.2")]
    public class VsColorOutputPackage : Package
    {
    }
}