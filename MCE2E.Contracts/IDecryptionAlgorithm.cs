using System.IO;

namespace MCE2E.Contracts
{
    internal interface IDecryptionAlgorithm
    {
        FileInfo Decrypt(FileInfo encryptedFile, byte[] key);

        byte[] DecryptSymmetricKey(byte[] encryptedKey, string privateKeyFilePath);
    }
}
