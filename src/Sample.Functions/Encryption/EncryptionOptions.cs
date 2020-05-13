using System;
using System.Security.Cryptography;
using System.Text;

namespace Sample.Functions.Encryption
{
    public class EncryptionOptions
    {
        public EncryptionOptions(string encryptionKey, Encoding encoding = null)
        {
            EncryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
            Encoding = encoding ?? Encoding.UTF8;
            using(var sha = new SHA256Managed())
            {
                EncryptionKeyHash = sha.ComputeHash(Encoding.GetBytes(encryptionKey));
            }
        }

        public string EncryptionKey { get; }
        public Encoding Encoding { get; }
        public byte[] EncryptionKeyHash { get; }
    }
}
