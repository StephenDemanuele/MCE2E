using System.IO;
using System.Security.Cryptography;

namespace MCE2E.Contracts
{
	public interface IStreamedEncryptionAlgorithm : IEncryptionAlgorithm
	{
		 void Encrypt(MemoryStream streamToEncrypt, CryptoStream cryptoStream);

		 ICryptoTransform GetEncryptor();
		 void Initialize(byte[] key);
	}
}
