using System;
using System.IO;
using System.Security.Cryptography;

namespace Sample.Functions.Encryption
{
    public class EncryptionService : IEncryptionService
    {
        private readonly EncryptionOptions _options;

        public EncryptionService(EncryptionOptions options)
        {
            _options = options;
        }

        public string Encrypt(string value)
        {
            //if null string was provided, nothing to encrypt
            if (string.IsNullOrEmpty(value))
                return value;

            var buffer = _options.Encoding.GetBytes(value);
            using (var input = new MemoryStream(buffer, false))
            using (var output = new MemoryStream())
            using (var aes = new AesManaged { Key = _options.EncryptionKeyHash })
            {
                var iv = aes.IV;
                output.Write(iv, 0, iv.Length);
                output.Flush();

                var encryptor = aes.CreateEncryptor(_options.EncryptionKeyHash, iv);
                using (var crypto = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
                {
                    input.CopyTo(crypto);
                }

                return Convert.ToBase64String(output.ToArray());
            }
        }

        public string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            try
            {
                var buffer = Convert.FromBase64String(value);

                using (var inputStream = new MemoryStream(buffer, false))
                using (var outputStream = new MemoryStream())
                using (var aes = new AesManaged { Key = _options.EncryptionKeyHash })
                {
                    var iv = aes.IV;
                    var bytesRead = inputStream.Read(iv, 0, iv.Length);
                    if (bytesRead < iv.Length)
                    {
                        throw new CryptographicException("IV is missing or invalid.");
                    }

                    var decryptor = aes.CreateDecryptor(_options.EncryptionKeyHash, iv);
                    using (var cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(outputStream);
                    }

                    return _options.Encoding.GetString(outputStream.ToArray());
                }
            }
            catch
            {
                // if we got an exception, likely this is due to the field not yet being encrypted
                // or it was encrypted by some other means and therefore cannot be decrypted here.
                // return the value given
                return value;
            }
        }
    }
}
