namespace ClassifiedsApp.Application.Interfaces.Services.Common;

public interface IEncryptionService
{
	string Encrypt(string plainText);
	string Decrypt(string cipherText);
}
