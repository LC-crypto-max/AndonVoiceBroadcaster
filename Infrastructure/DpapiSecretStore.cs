using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AndonVoiceBroadcaster.Infrastructure
{
    public sealed class DpapiSecretStore
    {
        public void Save(string path, string plaintext)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            byte[] source = Encoding.UTF8.GetBytes(plaintext ?? string.Empty);
            // 凭据仅允许当前 Windows 用户解密。
            byte[] encrypted = ProtectedData.Protect(source, null, DataProtectionScope.CurrentUser);
            File.WriteAllText(path, Convert.ToBase64String(encrypted), Encoding.UTF8);
        }

        public string Load(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            string value = File.ReadAllText(path, Encoding.UTF8);
            byte[] encrypted = Convert.FromBase64String(value);
            byte[] source = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(source);
        }
    }
}
