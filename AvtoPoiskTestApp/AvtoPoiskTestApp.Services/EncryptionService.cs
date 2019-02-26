using System;
using System.Security.Cryptography;
using System.Text;

namespace AvtoPoiskTestApp.Services
{
    public class EncryptionService
    {
        private const string Passphrase = "C70E53CD-31C2-4301-9309-60958BC32C93";
        public string Encrypt(string password)
        {
            byte[] results;
            var utf8 = new UTF8Encoding();
            var md5 = new MD5CryptoServiceProvider();
            var deskey = md5.ComputeHash(utf8.GetBytes(Passphrase));
            var desalg = new TripleDESCryptoServiceProvider
            {
                Key = deskey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            var encryptData = utf8.GetBytes(password);

            try
            {
                var encryptor = desalg.CreateEncryptor();
                results = encryptor.TransformFinalBlock(encryptData, 0, encryptData.Length);

            }
            finally
            {
                desalg.Clear();
                md5.Clear();
            }
            return Convert.ToBase64String(results);
        }

        public string Decrypt(string encryptedPassword)
        {
            byte[] results;
            var utf8 = new UTF8Encoding();
            var md5 = new MD5CryptoServiceProvider();
            var deskey = md5.ComputeHash(utf8.GetBytes(Passphrase));
            var desalg = new TripleDESCryptoServiceProvider
            {
                Key = deskey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            var decryptData = Convert.FromBase64String(encryptedPassword);
            try
            {
                var decryptor = desalg.CreateDecryptor();
                results = decryptor.TransformFinalBlock(decryptData, 0, decryptData.Length);
            }
            finally
            {
                desalg.Clear();
                md5.Clear();
            }
            return utf8.GetString(results);
        }
    }
}
