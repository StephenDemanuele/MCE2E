﻿using System;
using System.IO;
using System.Text;
using MCE2E.Contracts;
using System.Security.Cryptography;

namespace MCE2E.AESEncryption
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

		public ICryptoTransform InitializeEncryption(byte[] key, Stream targetStream)
		{
			var aes = Aes.Create();
			aes.Key = key;
			aes.GenerateIV();
			//write IV at start of target stream. must be written to targetStream so it is unencrypted
			targetStream.Write(aes.IV, 0, aes.IV.Length);

			return aes.CreateEncryptor();
		}

		public ICryptoTransform InitializeDecryption(byte[] key, Stream sourceStream)
		{
			var aes = Aes.Create();
			var iv = new byte[aes.IV.Length];
			sourceStream.Read(iv, 0, iv.Length);
			var cryptoTransform = aes.CreateDecryptor(key, iv);

			return cryptoTransform;
		}
		
		public FileInfo Decrypt(FileInfo encryptedFile, byte[] symmetricKey)
		{
			var decryptedStreamPath = Path.Combine(encryptedFile.DirectoryName,
				encryptedFile.Name.Replace(".enc", string.Empty));
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
