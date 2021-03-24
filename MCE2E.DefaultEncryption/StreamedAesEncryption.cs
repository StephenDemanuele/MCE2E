using System;
using System.IO;
using System.Net.Sockets;
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

		public void Initialize(byte[] key, Stream targetStream)
		{
			_aes.Key = key;
			_aes.GenerateIV();
			//write IV at start of target stream. must be written to targetStream so it is unencrypted
			targetStream.Write(_aes.IV, 0, _aes.IV.Length);
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
