// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;

namespace BlueOnionSoftware
{
    [Guid("CD56B219-38CB-482A-9B2D-7582DF4AAF1E")]
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\10.0")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideOptionPage(typeof(VsColorOutputOptions), VsColorOutputOptions.Category, VsColorOutputOptions.SubCategory, 1000, 1001, true)]
    [ProvideProfile(typeof(VsColorOutputOptions), VsColorOutputOptions.Category, VsColorOutputOptions.SubCategory, 1000, 1001, true)]
    [InstalledProductRegistration("VSColorOutput", "Color output for build and debug windows", "VSColorOutput - Blue Onion Software")]
    public class VsColorOutputPackage : Package
    {
    }
}