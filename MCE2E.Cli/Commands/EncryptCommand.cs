using System.IO;
using GoCommando;
using MCE2E.Controller.Services;
using MCE2E.Controller.Exceptions;

namespace MCE2E.Cli.Commands
{
    [Description("Encrypt files in a specified directory")]
    [Command("encrypt", "crypto")]
    public class EncryptCommand : BaseCommand
    {
        [Parameter("targetdir")]
        [Description("The directory containing the files to encrypt.")]
        public override string TargetDirectory { get; set; }

        [Parameter("ext")]
        [Description("The extension of files to encrypt.")]
        public string TargetFileExtension { get ;set;}

        public override void Run()
        {
            if (!ValidateArguments())
            {
                return;
            }

            var _encryptionService = new DefaultEncryptionService();
            try
            {
                _encryptionService.Initialize(PluginDirectory);
                var targetDir = new DirectoryInfo(TargetDirectory);
                var targetFiles = targetDir.GetFiles(TargetFileExtension);
                Log($"Found {targetFiles.Length} to encrypt.");

                foreach(var file in targetFiles)
                {
                    Log($"Encrypting {file.Name}");
                    var encryptedDirectory = _encryptionService.Encrypt(file);
                    Log($"Encrypted file added to {encryptedDirectory.FullName}");
                }

            }
            catch (EncryptionServiceBootstrappingException encryptionServiceBootstrappingException)
            {
                Log("Error bootstrapping", isError: true);
                Log(encryptionServiceBootstrappingException.InnerException.Message, true);
            }
        }
    }
}
