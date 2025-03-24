namespace MyDigimal.Common.Cryptography
{
    public interface IEncryptor
    {
        string Encrypt(string source);
        string Decrypt(string source);
    }
}