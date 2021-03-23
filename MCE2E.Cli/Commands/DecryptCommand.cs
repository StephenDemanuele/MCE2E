using System.IO;
using GoCommando;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.Cli.Commands
{
    [Description("Decrypt files in a specified directory")]
    [Command("decrypt", "crypto")]
    public class DecryptCommand : BaseCommand
    {
        [Parameter("targetdir", "td")]
        [Description("The directory where decrypted files should be placed.")]
        public string TargetDirectory { get; set; }

        [Parameter("sourcedir", "sd")]
        [Description("The directory where encrypted files are.")]
        public string SourceDirectory{ get; set; }

        [Parameter("pkf")]
        [Description("The full path to the private key file.")]
        public string PrivateKeyFile { get; set; }

        public override void Run()
        {
            Initialize();
            var decryptionService = ServiceProvider.GetService<IDecryptionService>();
            try
            {
                var dir = new DirectoryInfo(TargetDirectory);
                Log($"Decrypting files in {dir.FullName}", LogLevel.Info);

                // ReSharper disable PossibleNullReferenceException
                var decryptedFiles = decryptionService.Decrypt(
                    dir,
                    new FileInfo(PrivateKeyFile));
                decryptedFiles.ForEach(x => Log($"Decrypted {x.FullName}", LogLevel.Info));
            }
            catch (EncryptionServiceBootstrappingException encryptionServiceBootstrappingException)
            {
                Log("Error bootstrapping", LogLevel.Error);
                Log(encryptionServiceBootstrappingException.InnerException.Message, LogLevel.Error);
            }
        }

    }
}
