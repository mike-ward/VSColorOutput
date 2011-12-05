// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System;
using System.Runtime.InteropServices;
using BlueOnionSoftware;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class TextManagerEventsTests
    {
        [Test]
        public void UserPrefrencesChangedUpdateColorsForTextEditorCategory()
        {
            var textManagerEvents = new TextManagerEvents();
            var fontResource = new FONTCOLORPREFERENCES
            {
                pguidFontCategory = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Guid))),
                pguidColorService = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Guid))),
                pColorTable = Marshal.AllocHGlobal(1)
            };
            Marshal.StructureToPtr(DefGuidList.guidTextEditorFontCategory, fontResource.pguidFontCategory, true);
            Marshal.StructureToPtr(Guid.NewGuid(), fontResource.pguidColorService, true);
            var fontResources = new[] { fontResource };
            try
            {
                var mockStore = new Mock<IVsFontAndColorStorage>();
                FontAndColorStorage.Override = mockStore.Object;
                textManagerEvents.OnUserPreferencesChanged(null, null, null, fontResources);

                const uint flags = (uint)(
                    __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS |
                        __FCSTORAGEFLAGS.FCSF_NOAUTOCOLORS |
                            __FCSTORAGEFLAGS.FCSF_PROPAGATECHANGES);

                var textEditorGuid = DefGuidList.guidTextEditorFontCategory;
                mockStore.Verify(s => s.OpenCategory(ref textEditorGuid, flags));
            }
            finally
            {
                FontAndColorStorage.Override = null;
            }
        }

        [Test]
        public void UserPrefrencesChangedNotUpdateColorsForNotTextEditorCategory()
        {
            try
            {
                var textManagerEvents = new TextManagerEvents();
                var fontResource = new FONTCOLORPREFERENCES
                {
                    pguidFontCategory = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Guid))),
                    pguidColorService = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Guid))),
                    pColorTable = Marshal.AllocHGlobal(1)
                };
                Marshal.StructureToPtr(DefGuidList.guidPrinterFontCategory, fontResource.pguidFontCategory, true);
                Marshal.StructureToPtr(Guid.NewGuid(), fontResource.pguidColorService, true);
                var fontResources = new[] { fontResource };
                var mockStore = new Mock<IVsFontAndColorStorage>();
                FontAndColorStorage.Override = mockStore.Object;
                textManagerEvents.OnUserPreferencesChanged(null, null, null, fontResources);

                const uint flags = (uint)(
                    __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS |
                        __FCSTORAGEFLAGS.FCSF_NOAUTOCOLORS |
                            __FCSTORAGEFLAGS.FCSF_PROPAGATECHANGES);

                var textEditorGuid = DefGuidList.guidTextEditorFontCategory;
                mockStore.Verify(s => s.OpenCategory(ref textEditorGuid, flags), Times.Never());
            }
            finally
            {
                FontAndColorStorage.Override = null;
            }
        }
    }
}