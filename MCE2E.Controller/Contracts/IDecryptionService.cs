using System.IO;
using System.Collections.Generic;

namespace MCE2E.Controller.Contracts
{
    internal interface IDecryptionService
    {
        void Initialize(string pluginDirectory);

        List<FileInfo> Decrypt(DirectoryInfo directory, FileInfo privateKeyFile);
    }
}
