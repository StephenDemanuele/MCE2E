using System.IO;
using MCE2E.Contracts;
using System.Threading;
using System.Threading.Tasks;
using MCE2E.Controller.Contracts;
using System.Security.Cryptography;

namespace MCE2E.Controller.Services
{
	public class StreamedEncryptionService : IStreamedEncryptionService
	{

		private readonly IConfiguration _configuration;
		private readonly IStreamedEncryptionAlgorithm _encryptionAlgorithm;
		private readonly IKeyFactory _keyFactory;

		public StreamedEncryptionService(
			IConfigurationService configurationService,
			IStreamedEncryptionAlgorithm encryptionAlgorithm,
			IKeyFactory keyFactory)
		{
			_configuration = configurationService.Get();
			_encryptionAlgorithm = encryptionAlgorithm;
			_keyFactory = keyFactory;
		}

		public Task EncryptAsync(
			FileInfo sourceFileToEncrypt,
			string targetLocation,
			CancellationToken cancellationToken)
		{
			//add sanity checks on arguments
			var targetFilepath = Path.Combine(targetLocation, $"{sourceFileToEncrypt}.enc");

			var symmetricKey = _keyFactory.Get(16);
			using (var targetStream = new FileStream(targetFilepath, FileMode.CreateNew))
			{
				_encryptionAlgorithm.Initialize(symmetricKey, targetStream);
				using (var cryptoStream = new CryptoStream(targetStream, _encryptionAlgorithm.GetEncryptor(), CryptoStreamMode.Write))
				{
					using (var cryptoStreamWriter = new StreamWriter(cryptoStream))
					{
						using (var sourceFileStreamReader = new StreamReader(sourceFileToEncrypt.FullName))
						{
							var chunkSize = 1024;
							var buffer = new char[chunkSize];
							int bytesRead;
							while ((bytesRead = sourceFileStreamReader.Read(buffer, 0, buffer.Length)) > 0)
							{
								cryptoStreamWriter.Write(buffer, 0, bytesRead);
							}
							sourceFileStreamReader.Close();
						}
						cryptoStreamWriter.Close();
					}
					cryptoStream.Close();
				}
				targetStream.Close();
			}

			//encrypt encryptedSymmetricKey and save with file
			var encryptedSymmetricKeyFilepath = Path.Combine(targetLocation, $"{sourceFileToEncrypt}.enc.key");
			var publicKeyFilePath = _configuration.PathToPublicKey;
			var encryptedSymmetricKey = _encryptionAlgorithm.EncryptSymmetricKey(symmetricKey, publicKeyFilePath);
			File.WriteAllBytes(encryptedSymmetricKeyFilepath, encryptedSymmetricKey);
			return Task.CompletedTask;
		}
	}
}
