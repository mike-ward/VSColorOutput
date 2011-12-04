// Copyright (c) 2011 Blue Onion Software, All rights reserved
using BlueOnionSoftware;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class FontAndColorStorageTests
    {
        [Test]
        public void UpdateColorsUpdatesOutputWindowCategory()
        {
            try
            {
                var mockStore = new Mock<IVsFontAndColorStorage>();
                FontAndColorStorage.Override = mockStore.Object;
                FontAndColorStorage.UpdateColors();

                //const uint flags = (uint)(
                //    __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS |
                //        __FCSTORAGEFLAGS.FCSF_NOAUTOCOLORS |
                //            __FCSTORAGEFLAGS.FCSF_PROPAGATECHANGES);

                //mockStore.Verify(s => s.OpenCategory(DefGuidList.guidTextEditorFontCategory, flags));
            }
            finally
            {
                FontAndColorStorage.Override = null;
            }
        }
    }
}