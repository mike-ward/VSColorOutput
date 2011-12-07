// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace BlueOnionSoftware
{
    [Guid("CD56B219-38CB-482A-9B2D-7582DF4AAF1E")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideOptionPage(typeof(VsColorOutputOptions), "VSColorOutput", "Options", 1000, 1001, false)]
    [InstalledProductRegistration("VSColorOutput", "Color output for build and debug windows", "VSColorOutput - Blue Onion Software")]
    public class VsColorOutputPackage : Package
    {
        public VsColorOutputPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this));
        }

        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this));
            base.Initialize();
        }
    }
}