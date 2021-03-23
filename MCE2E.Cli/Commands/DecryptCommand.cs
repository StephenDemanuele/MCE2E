using System.IO;
using GoCommando;
using MCE2E.Controller.Services;
using MCE2E.Controller.Exceptions;

namespace MCE2E.Cli.Commands
{
    [Description("Decrypt files in a specified directory")]
    [Command("decrypt", "crypto")]
    public class DecryptCommand : BaseCommand
    {
        [Parameter("targetdir")]
        [Description("The directory containing the files to decrypt.")]
        public override string TargetDirectory { get; set; }

        [Parameter("pkf")]
        [Description("The full path to the private key file.")]
        public string PrivateKeyFile { get; set; }

        public override void Run()
        {
            if (!ValidateArguments())
            {
                return;
            }

            var _decryptionService = new DefaultDecryptionService();
            try
            {
                var dir = new DirectoryInfo(TargetDirectory);
                Log($"Decrypting files in {dir.FullName}");

                _decryptionService.Initialize(PluginDirectory);
                var decryptedFiles = _decryptionService.Decrypt(
                    dir,
                    new FileInfo(PrivateKeyFile));
                decryptedFiles.ForEach(x => Log($"Decrypted {x.FullName}"));
            }
            catch (EncryptionServiceBootstrappingException encryptionServiceBootstrappingException)
            {
                Log("Error bootstrapping", isError: true);
                Log(encryptionServiceBootstrappingException.InnerException.Message, isError: true);
            }
        }

    }
}
