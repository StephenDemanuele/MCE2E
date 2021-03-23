using System.IO;
using MCE2E.Contracts;
using System.Collections.Generic;
using MCE2E.Controller.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.Controller.Services
{
    /// <summary>
    /// This service is consumed by the test application to verify encryption is working correctly.
    /// This means the dependency DefaultEncryptionService <--> IEncryptionService has to be registered
    /// with the host's DI container.
    /// <see cref="Initialize()"/> must be called before use.
    /// </summary>
    internal class DefaultDecryptionService : BaseCryptographyService, IDecryptionService
    {
        public List<FileInfo> Decrypt(DirectoryInfo directory, FileInfo privateKeyFile)
        {
            var filepairs = GetPairs(directory);

            var result = new List<FileInfo>();
            foreach (var filepair in filepairs)
            {
                var decryptedFile = DecryptFilePair(filepair, privateKeyFile);
                result.Add(decryptedFile);
            }

            return result;
        }

        /// <summary>
        /// Uses key file, encrypted file and private key (from private key file), decrypts file.
        /// </summary>
        /// <param name="encryptedFileKeyPair"></param>
        /// <param name="privateKeyFile"></param>
        /// <returns>The decrypted file.</returns>
        private FileInfo DecryptFilePair(EncryptedFileKeyPair encryptedFileKeyPair, FileInfo privateKeyFile)
        {
            var symmetricKey = DecryptSymmetricKey(encryptedFileKeyPair.KeyFile, privateKeyFile);
            var decryptor = _serviceProvider.GetService<IDecryptionAlgorithm>();

            return decryptor.Decrypt(encryptedFileKeyPair.EncryptedFile, symmetricKey);
        }

        private byte[] DecryptSymmetricKey(FileInfo encryptedSymmetricKeyFile, FileInfo privateKeyFile)
        {
            var encryptor = _serviceProvider.GetService<IEncryptionAlgorithm>();

            //open .key file, read key
            var encryptedKey = File.ReadAllBytes(encryptedSymmetricKeyFile.FullName);
            //decrypt symmetric key using private key
            var decryptedKey = encryptor.DecryptSymmetricKey(encryptedKey, privateKeyFile);

            return decryptedKey;
        }

        private List<EncryptedFileKeyPair> GetPairs(DirectoryInfo directoryInfo)
        {
            var keyFiles = directoryInfo.GetFiles("*.key");
            var filePairs = new List<EncryptedFileKeyPair>();

            foreach (var keyFile in keyFiles)
            {
                var encryptedFileName = $"{keyFile.Name.Replace(".key", string.Empty)}.enc";
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
