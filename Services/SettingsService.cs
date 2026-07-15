using System;
using System.IO;
using System.Web.Script.Serialization;
using AndonVoiceBroadcaster.Infrastructure;
using AndonVoiceBroadcaster.Models;

namespace AndonVoiceBroadcaster.Services
{
    public sealed class SettingsService
    {
        private readonly JavaScriptSerializer _json = new JavaScriptSerializer();
        private readonly DpapiSecretStore _secretStore = new DpapiSecretStore();

        public SettingsService(string applicationDirectory)
        {
            ApplicationDirectory = applicationDirectory;
            SettingsPath = Path.Combine(applicationDirectory, "config", "appsettings.json");
            CredentialsPath = Path.Combine(applicationDirectory, "data", "credentials.dat");
        }

        public string ApplicationDirectory { get; private set; }
        public string SettingsPath { get; private set; }
        public string CredentialsPath { get; private set; }

        public AppSettings LoadSettings()
        {
            if (!File.Exists(SettingsPath))
            {
                return new AppSettings();
            }

            var settings = _json.Deserialize<AppSettings>(File.ReadAllText(SettingsPath));
            settings = settings ?? new AppSettings();
            settings.Normalize();
            return settings;
        }

        public void SaveSettings(AppSettings settings)
        {
            settings.Normalize();
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
            File.WriteAllText(SettingsPath, _json.Serialize(settings));
        }

        public MesCredentials LoadCredentials()
        {
            try
            {
                string plaintext = _secretStore.Load(CredentialsPath);
                return string.IsNullOrWhiteSpace(plaintext)
                    ? new MesCredentials()
                    : _json.Deserialize<MesCredentials>(plaintext) ?? new MesCredentials();
            }
            catch
            {
                return new MesCredentials();
            }
        }

        public void SaveCredentials(MesCredentials credentials)
        {
            _secretStore.Save(CredentialsPath, _json.Serialize(credentials ?? new MesCredentials()));
        }
    }
}
