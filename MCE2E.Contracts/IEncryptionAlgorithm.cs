using System.IO;

namespace MCE2E.Contracts
{
	public interface IEncryptionAlgorithm
	{
		byte[] EncryptSymmetricKey(byte[] key, string pathToPublicKey);

		FileInfo Encrypt(byte[] key, FileInfo fileToEncrypt, DirectoryInfo targetDirectory);
	}
}
