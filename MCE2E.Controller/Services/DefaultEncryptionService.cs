using System.IO;
using System.Text;
using MCE2E.Contracts;
using MCE2E.Controller.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.Controller.Services
{
    /// <summary>
    /// This service is consumed by the host application.
    /// This means the dependency DefaultEncryptionService <--> IEncryptionService has to be registered
    /// with the host's DI container.
    /// <see cref="Initialize()"/> must be called before use.
    /// </summary>
    internal class DefaultEncryptionService : BaseCryptographyService, IEncryptionService
    {
        public DirectoryInfo Encrypt(FileInfo fileToEncrypt)
        {
            var encryptionAlgorithm = _serviceProvider.GetService<IEncryptionAlgorithm>();
            var keyFactory = _serviceProvider.GetService<IKeyFactory>();
            var configuration = _serviceProvider.GetService<IConfiguration>();

            var symmetricKey = keyFactory.Get(16);
            
            var encryptedFile = encryptionAlgorithm.Encrypt(symmetricKey, fileToEncrypt);
            
            //create encryptedSymmetricKey and save with file
            var encryptedSymmetricKey = encryptionAlgorithm.EncryptSymmetricKey(symmetricKey, configuration.PathToPublicKey);

            var encryptedSymmetricKeyFilePath = Path.Combine(fileToEncrypt.DirectoryName, "Encrypted", $"{fileToEncrypt.Name}.key");
            File.WriteAllBytes(encryptedSymmetricKeyFilePath, encryptedSymmetricKey);
            var keyFile = new FileInfo(encryptedSymmetricKeyFilePath);

            return keyFile.Directory;
        }
    }
}
