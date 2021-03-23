using System;
using System.IO;
using System.Text;
using MCE2E.Contracts;
using System.Security.Cryptography;

namespace MCE2E.DefaultEncryption
{
	public class AESEncryption : IEncryptionAlgorithm, IDecryptionAlgorithm
	{
		/// <summary>
		/// Given <paramref name="key"/>, encrypts it using <paramref name="pathToPublicKey"/> to a public key file.
		/// </summary>
		/// <param name="key">The key to encrypt.</param>
		/// <param name="pathToPublicKey">Path to the public key which will be used to encrypt key.</param>
		/// <returns>Encrypted key.</returns>
		/// <exception cref="ArgumentNullException">For <paramref name="key"/> and <paramref name="pathToPublicKey"/></exception>
		/// <exception cref="FileNotFoundException">When file does not exist at <paramref name="pathToPublicKey"/></exception>
		public byte[] EncryptSymmetricKey(byte[] key, string pathToPublicKey)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (string.IsNullOrEmpty((pathToPublicKey)))
			{
				throw new ArgumentNullException(nameof(pathToPublicKey));
			}

			if (!File.Exists(pathToPublicKey))
			{
				throw new FileNotFoundException("Missing public key file", pathToPublicKey);
			}

			var rsaOpenSsl = RSA.Create();
			var publicKeyXmlString = File.ReadAllText(pathToPublicKey);
			rsaOpenSsl.FromXmlString(publicKeyXmlString);

			return rsaOpenSsl.Encrypt(key, RSAEncryptionPadding.OaepSHA1);
		}

		/// <summary>
		/// Given <paramref name="encryptedKey"/>, decrypts it using <paramref name="privateKeyFilePath"/> to a private key file.
		/// </summary>
		/// <param name="encryptedKey">The encrypted key to decrypt.</param>
		/// <param name="privateKeyFilePath">Path to the private key which will be used to decrypt key.</param>
		/// <returns>Decrypted key.</returns>
		/// <exception cref="ArgumentNullException">For <paramref name="encryptedKey"/> and <paramref name="privateKeyFilePath"/></exception>
		/// <exception cref="FileNotFoundException">When file does not exist at <paramref name="privateKeyFilePath"/></exception>
		public byte[] DecryptSymmetricKey(byte[] encryptedKey, string privateKeyFilePath)
		{
			if (encryptedKey == null)
			{
				throw new ArgumentNullException(nameof(encryptedKey));
			}

			if (string.IsNullOrEmpty((privateKeyFilePath)))
			{
				throw new ArgumentNullException(nameof(privateKeyFilePath));
			}

			if (!File.Exists(privateKeyFilePath))
			{
				throw new FileNotFoundException("Missing private key file", privateKeyFilePath);
			}

			var rsaOpenSsl = RSA.Create();
			var privateKeyXmlString = File.ReadAllText(privateKeyFilePath);
			rsaOpenSsl.FromXmlString(privateKeyXmlString);
			var decryptedSymmetricKey = rsaOpenSsl.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA1);

			return decryptedSymmetricKey;
		}

		public FileInfo Encrypt(byte[] key, FileInfo fileToEncrypt, DirectoryInfo targetDirectory)
		{
			using (var aesAlg = Aes.Create())
			{
				aesAlg.Key = key;
				aesAlg.GenerateIV();

				var encryptedStreamPath = Path.Combine(targetDirectory.FullName, $"{fileToEncrypt.Name}.enc");
				if (!Directory.Exists(encryptedStreamPath))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(encryptedStreamPath));
				}

				using (var encryptedStream = new FileStream(encryptedStreamPath, FileMode.Create))
				{
					var cryptoTransform = aesAlg.CreateEncryptor();

					using (var cryptoStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Write))
					{
						encryptedStream.Write(aesAlg.IV, 0, aesAlg.IV.Length);

						using (var writer = new StreamWriter(cryptoStream))
						{
							using (var reader = new StreamReader(fileToEncrypt.FullName))
							{
								var chunkSize = 1024;
								var buffer = new char[chunkSize];
								int bytesRead;
								while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
								{
									writer.Write(buffer, 0, bytesRead);
								}
								reader.Close();
							}
							writer.Close();
						}
						cryptoStream.Close();
					}
					encryptedStream.Close();
				}

				return new FileInfo(encryptedStreamPath);
			}
		}

		public FileInfo Decrypt(FileInfo encryptedFile, byte[] symmetricKey)
		{
			var decryptedStreamPath = Path.Combine(encryptedFile.DirectoryName, encryptedFile.Name.Replace(".enc", string.Empty));
			using (var aesAlg = Aes.Create())
			{
				aesAlg.Key = symmetricKey;

				//reads encrypted data from encrypted file
				using (var encryptedStream = new FileStream(encryptedFile.FullName, FileMode.Open))
				{
					var iv = new byte[aesAlg.IV.Length];
					encryptedStream.Read(iv, 0, iv.Length);
					var cryptoTransform = aesAlg.CreateDecryptor(symmetricKey, iv);

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
			}

			return new FileInfo(decryptedStreamPath);
		}
	}
}
