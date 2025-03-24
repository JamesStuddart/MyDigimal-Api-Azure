using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using MyDigimal.Common.Cryptography.Models;

namespace MyDigimal.Common.Cryptography
{
    public class Encryptor : IEncryptor
    {
        private readonly EncryptionConfig _encryptionConfig;

        public Encryptor(IOptions<EncryptionConfig> encryptionConfig)
        {
            _encryptionConfig = encryptionConfig.Value;
        }

        public string Encrypt(string source)
        {
            var input = Encoding.UTF8.GetBytes(source);
            var output = CreateDes().CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
            return Convert.ToBase64String(output);
        }

        public string Decrypt(string source)
        {
            var input = Convert.FromBase64String(source);
            var output = CreateDes().CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
            return Encoding.UTF8.GetString(output);
        }

        private TripleDES CreateDes()
        {
            var md5 = new MD5CryptoServiceProvider();
            var des = new TripleDESCryptoServiceProvider();
            var desKey = md5.ComputeHash(Encoding.UTF8.GetBytes(_encryptionConfig.Key));
            des.Key = desKey;
            des.IV = new byte[des.BlockSize / 8];
            des.Padding = PaddingMode.PKCS7;
            des.Mode = CipherMode.ECB;
            return des;
        }
    }
}