using System.Diagnostics;

namespace VSColorOutput.State
{
    internal static class Log
    {
        internal static void LogError(string message)
        {
            try
            {
                // I'm co-opting the Visual Studio event source because I can't register
                // my own from a .VSIX installer.
                EventLog.WriteEntry("Microsoft Visual Studio",
                    "VSColorOutput: " + (message ?? "null"),
                    EventLogEntryType.Error);
            }
            catch
            {
                // Don't kill extension for logging errors
            }
        }
    }
}