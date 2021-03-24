using System;
using System.IO;
using MCE2E.Contracts;
using System.Security.Cryptography;

namespace MCE2E.DefaultEncryption
{
	public class StreamedAesEncryption : AESEncryption, IStreamedEncryptionAlgorithm, IDisposable
	{
		private readonly Aes _aes;

		public StreamedAesEncryption()
		{
			_aes = Aes.Create();
		}

		public void Initialize(byte[] key)
		{
			_aes.Key = key;
			_aes.GenerateIV();

			//at which point does this need to be written?
			//definitely at start of stream;
			//which stream?

		}

		public void Encrypt(MemoryStream streamToEncrypt, CryptoStream cryptoStream)
		{
			throw new System.NotImplementedException();
		}
		
		public ICryptoTransform GetEncryptor()
		{
			return _aes.CreateEncryptor();
		}

		public void Dispose()
		{
			_aes.Dispose();
		}
	}
}
