namespace AvtoPoiskTestApp.Services.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string password);
        string Decrypt(string encryptedPassword);
    }
}
