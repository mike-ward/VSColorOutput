// Copyright (c) 2011 Blue Onion Software, All rights reserved
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace BlueOnionSoftware
{
    public class TextManagerEvents : IVsTextManagerEvents
    {
        private Guid _guidColorService = Guid.Empty;
        public static IVsTextManager2 Override;

        private static IVsTextManager2 GetService()
        {
            return Override ?? ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager)) as IVsTextManager2;
        }

        public static void RegisterForTextManagerEvents()
        {
            var textManager = GetService();
            var container = textManager as IConnectionPointContainer;
            IConnectionPoint textManagerEventsConnection;
            var eventGuid = typeof(IVsTextManagerEvents).GUID;
            container.FindConnectionPoint(ref eventGuid, out textManagerEventsConnection);
            var textManagerEvents = new TextManagerEvents();
            uint textManagerCookie;
            textManagerEventsConnection.Advise(textManagerEvents, out textManagerCookie);

            FontAndColorStorage.UpdateColors(); //Hotfix for loading colors until visual studio call OnUserPreferencesChanged function
        }

        public void OnRegisterMarkerType(int iMarkerType)
        {
        }

        public void OnRegisterView(IVsTextView pView)
        {
        }

        public void OnUnregisterView(IVsTextView pView)
        {
        }

        public void OnUserPreferencesChanged(
            VIEWPREFERENCES[] pViewPrefs,
            FRAMEPREFERENCES[] pFramePrefs,
            LANGPREFERENCES[] pLangPrefs,
            FONTCOLORPREFERENCES[] pColorPrefs)
        {
            if (pColorPrefs != null && pColorPrefs.Length > 0 && pColorPrefs[0].pColorTable != null)
            {
                var guidFontCategory = (Guid)Marshal.PtrToStructure(pColorPrefs[0].pguidFontCategory, typeof(Guid));
                var guidColorService = (Guid)Marshal.PtrToStructure(pColorPrefs[0].pguidColorService, typeof(Guid));
                if (_guidColorService == Guid.Empty)
                {
                    _guidColorService = guidColorService;
                }
                if (guidFontCategory == DefGuidList.guidTextEditorFontCategory && _guidColorService == guidColorService)
                {
                    FontAndColorStorage.UpdateColors();
                }
            }
        }
    }
}