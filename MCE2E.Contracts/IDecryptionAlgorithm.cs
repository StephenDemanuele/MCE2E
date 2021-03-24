using System.IO;
using System.Security.Cryptography;

namespace MCE2E.Contracts
{
    internal interface IDecryptionAlgorithm
    {
        FileInfo Decrypt(FileInfo encryptedFile, byte[] key);

        ICryptoTransform InitializeDecryption(byte[] key, Stream sourceStream);
    }
}
