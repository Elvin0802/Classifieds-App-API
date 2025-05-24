using ClassifiedsApp.Application.Dtos.Encryption;
using ClassifiedsApp.Application.Interfaces.Services.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedsApp.Infrastructure.Services.Encryption;

public class EncryptionService : IEncryptionService
{
	readonly EncryptionSettingsDto _settings;

	public EncryptionService(IOptions<EncryptionSettingsDto> settings)
	{
		_settings = settings.Value;
	}

	public string Encrypt(string plainText)
	{
		if (string.IsNullOrEmpty(plainText))
			return plainText;

		try
		{
			using var aes = Aes.Create();
			aes.Key = Convert.FromBase64String(_settings.Key);
			aes.GenerateIV();

			var encryptor = aes.CreateEncryptor();
			var plainBytes = Encoding.UTF8.GetBytes(plainText);
			var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

			var result = new byte[aes.IV.Length + encryptedBytes.Length];
			Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
			Array.Copy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

			return Convert.ToBase64String(result);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("Encryption failed", ex);
		}
	}

	public string Decrypt(string cipherText)
	{
		if (string.IsNullOrEmpty(cipherText))
			return cipherText;

		try
		{
			var fullCipher = Convert.FromBase64String(cipherText);

			using var aes = Aes.Create();
			aes.Key = Convert.FromBase64String(_settings.Key);

			var iv = new byte[aes.IV.Length];
			var cipher = new byte[fullCipher.Length - iv.Length];

			Array.Copy(fullCipher, 0, iv, 0, iv.Length);
			Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

			aes.IV = iv;

			var decryptor = aes.CreateDecryptor();
			var decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

			return Encoding.UTF8.GetString(decryptedBytes);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("Decryption failed", ex);
		}
	}
}
