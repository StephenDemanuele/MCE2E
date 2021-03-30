using System.IO;
using MCE2E.Contracts;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MCE2E.Controller.Contracts;

namespace MCE2E.Controller.Services
{
	/// <summary>
	/// This service is consumed by the test application to verify encryption is working correctly.
	/// </summary>
	internal class DefaultDecryptionService : IDecryptionService
	{
		private readonly IEncryptionAlgorithm _encryptionAlgorithm;
		private readonly IDecryptionAlgorithm _decryptionAlgorithm;

		public DefaultDecryptionService(
			IEncryptionAlgorithm encryptionAlgorithm, 
			IDecryptionAlgorithm decryptionAlgorithm)
		{
			_encryptionAlgorithm = encryptionAlgorithm;
			_decryptionAlgorithm = decryptionAlgorithm;
		}

		public List<FileInfo> Decrypt(DirectoryInfo directory, FileInfo privateKeyFile)
		{
			var filePairs = GetPairs(directory);

			var result = new List<FileInfo>();
			foreach (var filePair in filePairs)
			{
				var decryptedFile = DecryptFilePair(filePair, privateKeyFile.FullName);
				result.Add(decryptedFile);
			}

			return result;
		}

		/// <summary>
		/// Uses key file, encrypted file and private key (from private key file), decrypts file.
		/// </summary>
		/// <param name="encryptedFileKeyPair">The pair of encrypted file and its key.</param>
		/// <param name="privateKeyFilePath">Fully qualified path to the private key.</param>
		/// <returns>The decrypted file.</returns>
		private FileInfo DecryptFilePair(EncryptedFileKeyPair encryptedFileKeyPair, string privateKeyFilePath)
		{
			var symmetricKey = DecryptSymmetricKey(encryptedFileKeyPair.KeyFile, privateKeyFilePath);

			var encryptedFile = encryptedFileKeyPair.EncryptedFile;
			var decryptedStreamPath = Path.Combine(encryptedFile.DirectoryName, encryptedFile.Name.Replace(".enc", string.Empty));
		
				//reads encrypted data from encrypted file
				using (var encryptedStream = new FileStream(encryptedFile.FullName, FileMode.Open))
				{
					var cryptoTransform = _decryptionAlgorithm.InitializeDecryption(symmetricKey, encryptedStream);

					//create a stream which reads the encrypted stream and applies the crypto-transform
					using (var cryptoStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Read))
					{
						using (var writer = new StreamWriter(decryptedStreamPath))
						{
							var chunkSize = 1024;
							var buffer = new byte[chunkSize];
							int bytesRead;
							while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
							{
								var x = Encoding.ASCII.GetString(buffer, 0, bytesRead);
								writer.Write(x);
							}

							writer.Close();
						}

						cryptoStream.Close();
					}

					encryptedStream.Close();
				}
			

			return new FileInfo(decryptedStreamPath);
			return _decryptionAlgorithm.Decrypt(encryptedFileKeyPair.EncryptedFile, symmetricKey);
		}

		private byte[] DecryptSymmetricKey(FileInfo encryptedSymmetricKeyFile, string privateKeyFilePath)
		{
			//open .key file, read key
			var encryptedKey = File.ReadAllBytes(encryptedSymmetricKeyFile.FullName);
			//decrypt symmetric key using private key
			var decryptedKey = _encryptionAlgorithm.DecryptSymmetricKey(encryptedKey, privateKeyFilePath);

			return decryptedKey;
		}

		private List<EncryptedFileKeyPair> GetPairs(DirectoryInfo directoryInfo)
		{
			var keyFiles = directoryInfo.GetFiles("*.key");
			var filePairs = new List<EncryptedFileKeyPair>();

			foreach (var keyFile in keyFiles)
			{
				var encryptedFileName = keyFile.Name.Replace(".key", string.Empty);
				filePairs.Add(new EncryptedFileKeyPair(keyFile, new FileInfo(Path.Combine(keyFile.DirectoryName, encryptedFileName))));
			}

			return filePairs;
		}

		class EncryptedFileKeyPair
		{
			public EncryptedFileKeyPair(FileInfo keyFile, FileInfo encryptedFile)
			{
				KeyFile = keyFile;
				EncryptedFile = encryptedFile;
			}

			public FileInfo EncryptedFile { get; }

			public FileInfo KeyFile { get; }
		}
	}
}
