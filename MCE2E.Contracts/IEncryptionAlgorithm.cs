using System.IO;

namespace MCE2E.Contracts
{
	public interface IEncryptionAlgorithm
	{
		byte[] EncryptSymmetricKey(byte[] key, string pathToPublicKey);

		byte[] DecryptSymmetricKey(byte[] encryptedKey, FileInfo privateKeyFile);

		FileInfo Encrypt(byte[] key, FileInfo fileToEncrypt);
	}
}
