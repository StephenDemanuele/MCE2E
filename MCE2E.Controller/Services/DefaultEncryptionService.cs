using System.IO;
using MCE2E.Contracts;
using System.Threading.Tasks;
using MCE2E.Controller.Contracts;

namespace MCE2E.Controller.Services
{
	/// <summary>
	/// This service is consumed by the host application.
	/// </summary>
	internal class DefaultEncryptionService : IEncryptionService
	{
		private readonly IConfiguration _configuration;
		private readonly IEncryptionAlgorithm _encryptionAlgorithm;
		private readonly IKeyFactory _keyFactory;

		public DefaultEncryptionService(IConfigurationService configurationService, IEncryptionAlgorithm encryptionAlgorithm, IKeyFactory keyFactory)
		{
			_configuration = configurationService.Get();
			_encryptionAlgorithm = encryptionAlgorithm;
			_keyFactory = keyFactory;
		}

		public Task EncryptAsync(FileInfo fileToEncrypt, DirectoryInfo targetDirectoryInfo)
		{
			var symmetricKey = _keyFactory.Get(16);
			
			var encryptedFile = _encryptionAlgorithm.Encrypt(symmetricKey, fileToEncrypt, targetDirectoryInfo);
			
			//create encryptedSymmetricKey and save with file
			var encryptedSymmetricKey = _encryptionAlgorithm.EncryptSymmetricKey(symmetricKey, _configuration.PathToPublicKey);

			var encryptedSymmetricKeyFilePath = Path.Combine(targetDirectoryInfo.FullName, $"{fileToEncrypt.Name}.key");
			File.WriteAllBytes(encryptedSymmetricKeyFilePath, encryptedSymmetricKey);
			var keyFile = new FileInfo(encryptedSymmetricKeyFilePath);

			return Task.CompletedTask;
		}
	}
}
