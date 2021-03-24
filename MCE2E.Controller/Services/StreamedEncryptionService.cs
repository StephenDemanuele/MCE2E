using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using MCE2E.Contracts;
using System.Threading.Tasks;
using MCE2E.Controller.Contracts;

namespace MCE2E.Controller.Services
{
	public class StreamedEncryptionService : IStreamedEncryptionService
	{

		private readonly IConfiguration _configuration;
		private readonly IStreamedEncryptionAlgorithm _encryptionAlgorithm;
		private readonly IKeyFactory _keyFactory;

		public StreamedEncryptionService(IConfigurationService configurationService, IStreamedEncryptionAlgorithm encryptionAlgorithm, IKeyFactory keyFactory)
		{
			_configuration = configurationService.Get();
			_encryptionAlgorithm = encryptionAlgorithm;
			_keyFactory = keyFactory;
		}



		public Task EncryptAsync(
			FileInfo publicKeyFile, 
			DirectoryInfo targetDirectoryInfo, 
			CancellationToken cancellationToken)
		{
			var symmetricKey = _keyFactory.Get(16);
			_encryptionAlgorithm.Initialize(symmetricKey);
			
			var encryptor = _encryptionAlgorithm.GetEncryptor();

			//encrypt encryptedSymmetricKey and save with file
			var encryptedSymmetricKey = _encryptionAlgorithm.EncryptSymmetricKey(symmetricKey, publicKeyFile.FullName);
			
			return Task.CompletedTask;
		}
	}
}
