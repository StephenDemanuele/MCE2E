using System.IO;
using System.Security.Cryptography;

namespace MCE2E.Contracts
{
	public interface IEncryptionAlgorithm
	{
		byte[] EncryptSymmetricKey(byte[] key, string pathToPublicKey);

		byte[] DecryptSymmetricKey(byte[] encryptedKey, string privateKeyFilePath);
		
		ICryptoTransform InitializeEncryption(byte[] key, Stream targetStream);

		FileInfo Encrypt(byte[] key, FileInfo fileToEncrypt, DirectoryInfo targetDirectory);
	}
}
