namespace Sample.Functions.Encryption
{
    public interface IEncryptionService
    {
        string Decrypt(string value);
        string Encrypt(string value);
    }
}
