using System.IO;
using System.Linq;
using GoCommando;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.Cli.Commands
{
	[Description("Encrypt files in a specified directory")]
	[Command("encrypt", "crypto")]
	public class EncryptCommand : BaseCommand
	{
		[Parameter("targetdir")]
		[Description("The directory where encrypted files will go.")]
		public string TargetDirectory { get; set; }

		[Parameter("sourcedir", "sd")]
		[Description("The directory where original files are.")]
		public string SourceDirectory { get; set; }

		[Parameter("ext")]
		[Description("The extension of files to encrypt.")]
		public string TargetFileExtension { get ;set;}

		public override void Run()
		{
			Initialize();

			var encryptionService = ServiceProvider.GetService<IEncryptionService>();
			try
			{
				var sourceDir = new DirectoryInfo(SourceDirectory);
				if (!sourceDir.Exists)
				{
					Log($"Source dir {SourceDirectory} does not exist", LogLevel.Error);
					return;
				}

				var targetFiles = sourceDir.GetFiles(TargetFileExtension);
				if (!targetFiles.Any())
				{
					Log("No files found", LogLevel.Warn);
					return;
				}

				Log($"Found {targetFiles.Length} to encrypt.", LogLevel.Info);

				foreach(var file in targetFiles)
				{
					Log($"Encrypting {file.Name}", LogLevel.Info);
					// ReSharper disable once PossibleNullReferenceException
					encryptionService.EncryptAsync(file, new DirectoryInfo(TargetDirectory)).Wait();
					Log("Ready", LogLevel.Info);
				}

			}
			catch (EncryptionServiceBootstrappingException encryptionServiceBootstrappingException)
			{
				Log("Error bootstrapping", LogLevel.Error);
				Log(encryptionServiceBootstrappingException.InnerException.Message, LogLevel.Error);
			}
		}
	}
}
