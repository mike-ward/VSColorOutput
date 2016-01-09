using System.Collections.Generic;
using System.Drawing;
using VSColorOutput.State;

namespace VSColorOutput.Output.ColorClassifier
{
    public static class ColorMap
    {
        public static Dictionary<string, Color> GetMap()
        {
            var settings = Settings.Load();
            return new Dictionary<string, Color>
            {
                {ClassificationTypeDefinitions.BuildHead, settings.BuildMessageColor},
                {ClassificationTypeDefinitions.BuildText, settings.BuildTextColor},
                {ClassificationTypeDefinitions.LogError, settings.ErrorTextColor},
                {ClassificationTypeDefinitions.LogWarn, settings.WarningTextColor},
                {ClassificationTypeDefinitions.LogInfo, settings.InformationTextColor},
                {ClassificationTypeDefinitions.LogCustom1, settings.CustomTextColor1},
                {ClassificationTypeDefinitions.LogCustom2, settings.CustomTextColor2},
                {ClassificationTypeDefinitions.LogCustom3, settings.CustomTextColor3},
                {ClassificationTypeDefinitions.LogCustom4, settings.CustomTextColor4},
                {ClassificationTypeDefinitions.FindResultsFilename, settings.FindFileNameColor},
                {ClassificationTypeDefinitions.FindResultsSearchTerm, settings.FindSearchTermColor},
                {ClassificationTypeDefinitions.Timestamp, settings.TimestampColor}
            };
        }
    }
}