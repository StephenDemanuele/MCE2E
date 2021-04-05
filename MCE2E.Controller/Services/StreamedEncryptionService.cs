using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MCE2E.Contracts;
using System.Threading;
using System.Threading.Tasks;
using MCE2E.Controller.Contracts;
using System.Security.Cryptography;

namespace MCE2E.Controller.Services
{
	public class StreamedEncryptionService : IEncryptionService
	{

		private readonly IConfiguration _configuration;
		private readonly IEncryptionAlgorithm _encryptionAlgorithm;
		private readonly ISymmetricKeyProvider _keyFactory;
		private readonly IStreamProvider[] _streamProviders;

		public StreamedEncryptionService(
			IConfigurationService configurationService,
			IEncryptionAlgorithm encryptionAlgorithm,
			ISymmetricKeyProvider keyFactory,
			IEnumerable<IStreamProvider> streamProviders)
		{
			_configuration = configurationService.Get();
			_encryptionAlgorithm = encryptionAlgorithm;
			_keyFactory = keyFactory;
			_streamProviders = streamProviders.ToArray();
		}

		//TODO: use CancellationToken and check for cancellations
		public async Task EncryptAsync(
			FileInfo sourceFileToEncrypt,
			string targetLocation,
			TargetType targetType,
			CancellationToken cancellationToken)
		{
			var streamProvider = _streamProviders.First(x => x.Target == targetType);

			//add sanity checks on argument
			var targetFilepath = Path.Combine(targetLocation, $"{sourceFileToEncrypt.Name}.enc");
			var symmetricKey = _keyFactory.Get(16);

			if (!Directory.Exists(targetLocation))
			{
				Directory.CreateDirectory(targetLocation);
			}

			using (var targetStream = streamProvider.Get(targetFilepath))
			{
				var encryptor = _encryptionAlgorithm.InitializeEncryption(symmetricKey, targetStream);
				using (var cryptoStream = new CryptoStream(targetStream, encryptor, CryptoStreamMode.Write))
				{
					using (var cryptoStreamWriter = new StreamWriter(cryptoStream))
					{
						using (var sourceFileStreamReader = new StreamReader(sourceFileToEncrypt.FullName))
						{
							var chunkSize = 1024;
							var buffer = new char[chunkSize];
							int bytesRead;
							while ((bytesRead = await sourceFileStreamReader.ReadAsync(buffer, 0, buffer.Length)) > 0)
							{
								await cryptoStreamWriter.WriteAsync(buffer, 0, bytesRead);
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
			var encryptedSymmetricKeyFilepath = Path.Combine(targetLocation, $"{sourceFileToEncrypt.Name}.enc.key");
			var encryptedSymmetricKey = _encryptionAlgorithm.EncryptSymmetricKey(symmetricKey, _configuration.PathToPublicKey);
			File.WriteAllBytes(encryptedSymmetricKeyFilepath, encryptedSymmetricKey);
		}
	}
}
