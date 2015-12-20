using System;

namespace BlueOnionSoftware
{
    public class BuildEventsProvider
    {
        private static BuildEvents BuildEvents { get; set; }

        public static void ConstructBuildEvents(IServiceProvider serviceProvider)
        {
            if (BuildEvents == null)
            {
                BuildEvents = new BuildEvents(serviceProvider);
            }
        }
    }
}