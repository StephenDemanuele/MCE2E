using System;
using System.IO;
using GoCommando;
using System.Threading;
using MCE2E.Controller;
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
		public string TargetFileExtension { get; set; }

		public override void Run()
		{
			Initialize();
			var cancellationTokenSource = new CancellationTokenSource();
			var encryptionService = ServiceProvider.GetService<IEncryptionService>();

			encryptionService.OnProgressChanged += (sender, progress) =>
			{
				Log(progress.ToString(), LogLevel.Info);
			};

			Console.CancelKeyPress += (sender, args) =>
			{
				args.Cancel = true;
				Log("Cancellation requested.", LogLevel.Warn);
				cancellationTokenSource.Cancel();
			};

			try
			{
				var sourceDir = new DirectoryInfo(SourceDirectory);
				if (!sourceDir.Exists)
				{
					Log($"Source dir {SourceDirectory} does not exist", LogLevel.Error);
					return;
				}

				var sourceFiles = sourceDir.GetFiles(TargetFileExtension);
				var task = encryptionService.EncryptAsync(sourceFiles, TargetDirectory, TargetType.File, cancellationTokenSource.Token);
				task.Wait();
				var result = task.Result;

				Log("Ready", LogLevel.Info);
			}
			catch (EncryptionServiceBootstrappingException encryptionServiceBootstrappingException)
			{
				Log("Error occurred during bootstrapping", LogLevel.Error);
				Log(encryptionServiceBootstrappingException);
			}
			catch (Exception ex)
			{
				Log(ex);
			}
		}
	}
}
