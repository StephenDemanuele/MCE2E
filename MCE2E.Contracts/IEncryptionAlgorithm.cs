using System.IO;

namespace MCE2E.Contracts
{
	public interface IEncryptionAlgorithm
	{
		byte[] EncryptSymmetricKey(byte[] key, string pathToPublicKey);

		byte[] DecryptSymmetricKey(byte[] encryptedKey, string privateKeyFilePath);


		FileInfo Encrypt(byte[] key, FileInfo fileToEncrypt, DirectoryInfo targetDirectory);
	}
}
