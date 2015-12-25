using System.Collections.Generic;
using System.Drawing;

namespace BlueOnionSoftware
{
    public static class ColorMap
    {
        public static Dictionary<string, Color> GetMap()
        {
            var settings = Settings.Load();
            return new Dictionary<string, Color>
            {
                {OutputClassificationDefinitions.BuildHead, settings.BuildMessageColor},
                {OutputClassificationDefinitions.BuildText, settings.BuildTextColor},
                {OutputClassificationDefinitions.LogError, settings.ErrorTextColor},
                {OutputClassificationDefinitions.LogWarn, settings.WarningTextColor},
                {OutputClassificationDefinitions.LogInfo, settings.InformationTextColor},
                {OutputClassificationDefinitions.LogCustom1, settings.CustomTextColor1},
                {OutputClassificationDefinitions.LogCustom2, settings.CustomTextColor2},
                {OutputClassificationDefinitions.LogCustom3, settings.CustomTextColor3},
                {OutputClassificationDefinitions.LogCustom4, settings.CustomTextColor4},
                {OutputClassificationDefinitions.FindResultsFilename, settings.FindFileNameColor},
                {OutputClassificationDefinitions.FindResultsSearchTerm, settings.FindSearchTermColor}
            };
        }
    }
}