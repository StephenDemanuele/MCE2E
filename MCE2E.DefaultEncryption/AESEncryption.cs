using System.IO;
using MCE2E.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace MCE2E.DefaultEncryption
{
    public class AESEncryption : IEncryptionAlgorithm, IDecryptionAlgorithm
    {
        public byte[] EncryptSymmetricKey(byte[] key, string pathToPublicKey)
        {
            var rsaOpenSsl = RSAOpenSsl.Create();
            var publicKeyXmlString = File.ReadAllText(pathToPublicKey);
            rsaOpenSsl.FromXmlString(publicKeyXmlString);
            var encryptedSymmetricKey = rsaOpenSsl.Encrypt(key, RSAEncryptionPadding.OaepSHA1);

            return encryptedSymmetricKey;
        }

        public byte[] DecryptSymmetricKey(byte[] encryptedKey, FileInfo privateKeyFile)
        {
            var rsaOpenSsl = RSAOpenSsl.Create();
            var privateKeyXmlString = File.ReadAllText(privateKeyFile.FullName);
            rsaOpenSsl.FromXmlString(privateKeyXmlString);
            var decryptedSymmetricKey = rsaOpenSsl.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA1);

            return decryptedSymmetricKey;
        }

        public FileInfo Encrypt(byte[] key, FileInfo fileToEncrypt)
        {
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();

                // Create an encryptor to perform the stream transform.
                var encryptedStreamPath = Path.Combine(fileToEncrypt.DirectoryName, "Encrypted", $"{fileToEncrypt.Name}.enc");
                Directory.CreateDirectory(Path.Combine(fileToEncrypt.DirectoryName, "Encrypted"));
                using (var encryptedStream = new FileStream(encryptedStreamPath, FileMode.Create))
                {
                    var cryptoTransform = aesAlg.CreateEncryptor();

                    using (var cryptoStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        encryptedStream.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                        
                        using (var writer = new StreamWriter(cryptoStream))
                        {
                            using (var reader = new StreamReader(fileToEncrypt.FullName))
                            {
                                var chunkSize = 1024;
                                var buffer = new char[chunkSize];
                                int bytesRead;
                                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    writer.Write(buffer, 0, bytesRead);
                                }
                                reader.Close();
                            }
                            writer.Close();
                        }
                        cryptoStream.Close();
                    }
                    encryptedStream.Close();
                }

                return new FileInfo(encryptedStreamPath);
            }
        }

        public FileInfo Decrypt(FileInfo encryptedFile, byte[] symmetricKey)
        {
            var decryptedStreamPath = Path.Combine(encryptedFile.DirectoryName, encryptedFile.Name.Replace(".enc", string.Empty));
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = symmetricKey;

                //reads encrypted data from encrypted file
                using (var encryptedStream = new FileStream(encryptedFile.FullName, FileMode.Open))
                {
                    var iv = new byte[aesAlg.IV.Length];
                    encryptedStream.Read(iv, 0, iv.Length);
                    var cryptoTransform = aesAlg.CreateDecryptor(symmetricKey, iv);

                    //create a stream which reads the encrypted stream and applies the crypto-transform
                    using (var cryptoStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Read))
                    {
                        using (var writer = new StreamWriter(decryptedStreamPath))
                        {
                            var chunkSize = 1024;
                            var buffer = new byte[chunkSize];
                            int bytesRead;
                            while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                var x = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                                writer.Write(x);
                            }

                            writer.Close();
                        }

                        cryptoStream.Close();
                    }
                    encryptedStream.Close();
                }
            }

            return new FileInfo(decryptedStreamPath);
        }
    }
}
