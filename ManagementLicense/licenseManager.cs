using System.Text.Json;

namespace ManagementLicense
{
    public class licenseManager
    {
        private readonly HybridEncryptionService _crypto;

        public licenseManager(string rsaKey, bool isPrivateKey)
        {
            _crypto = new HybridEncryptionService(rsaKey, isPrivateKey);
        }

        public string GenerateLicense(object licenseObj)
        {
            string plainJson = JsonSerializer.Serialize(licenseObj);

            var (encryptedKey, encryptedData) = _crypto.EncryptLicense(plainJson);

            var package = new
            {
                Key = encryptedKey,
                Data = encryptedData
            };

            return JsonSerializer.Serialize(package);
        }

        public T ValidateLicense<T>(string LicenseJson)
        {
            var package = JsonSerializer.Deserialize<Dictionary<string, string>>(LicenseJson);

            string decrypted = _crypto.DecryptLicense(package["Key"], package["Data"]);

            return JsonSerializer.Deserialize<T>(decrypted);
        }
    }
}
