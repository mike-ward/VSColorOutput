// Copyright (c) 2012 Blue Onion Software. All rights reserved.

using System.Collections.Generic;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BlueOnionSoftware
{
    public static class FontAndColorStorage
    {
        public static IVsFontAndColorStorage Override { get; set; }

        public static IVsFontAndColorStorage GetFontAndColorStorageService()
        {
            return Override ?? Package.GetGlobalService(typeof(SVsFontAndColorStorage)) as IVsFontAndColorStorage;
        }

        private static readonly Dictionary<string, ColorableItemInfo[]> _colorMap = new Dictionary<string, ColorableItemInfo[]>
        {
            {OutputClassificationDefinitions.BuildHead, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.BuildText, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.LogInfo, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.LogWarn, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.LogError, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.LogCustom1, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.LogCustom2, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.LogCustom3, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.LogCustom4, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.FindResultsFilename, new[] {new ColorableItemInfo()}},
            {OutputClassificationDefinitions.FindResultsSearchTerm, new[] {new ColorableItemInfo()}}
        };

        private static volatile bool _isUpdatingColors;

        public static void UpdateColors()
        {
            if (_isUpdatingColors)
            {
                return;
            }

            _isUpdatingColors = true;

            const uint flags = (uint)(
                __FCSTORAGEFLAGS.FCSF_PROPAGATECHANGES |
                __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS |
                __FCSTORAGEFLAGS.FCSF_NOAUTOCOLORS);

            var store = GetFontAndColorStorageService();
            if (store != null)
            {
                try
                {
                    store.OpenCategory(DefGuidList.guidTextEditorFontCategory, flags);
                    foreach (var color in _colorMap)
                    {
                        store.GetItem(color.Key, color.Value);
                    }
                    store.CloseCategory();
                    store.OpenCategory(DefGuidList.guidOutputWindowFontCategory, flags);
                    foreach (var color in _colorMap)
                    {
                        store.SetItem(color.Key, color.Value);
                    }
                }
                finally
                {
                    store.CloseCategory();
                }
            }

            _isUpdatingColors = false;
        }
    }
}