using Microsoft.Win32;

namespace AndonVoiceBroadcaster.Services
{
    public sealed class StartupService
    {
        private const string RunKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string ValueName = "AndonVoiceBroadcaster";

        public void Configure(bool enabled, string executablePath)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKey, true))
            {
                if (key == null)
                {
                    return;
                }

                if (enabled)
                {
                    key.SetValue(ValueName, "\"" + executablePath + "\" --minimized");
                }
                else
                {
                    key.DeleteValue(ValueName, false);
                }
            }
        }
    }
}
