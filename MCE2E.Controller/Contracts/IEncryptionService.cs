using System.IO;

namespace MCE2E.Controller.Contracts
{
    internal interface IEncryptionService
    {
        void Initialize(string pluginDirectory);

        DirectoryInfo Encrypt(FileInfo filepublicKeyFile);
    }
}
