using System;
using System.IO;
using GoCommando;
using System.Threading;
using MCE2E.Controller;
using System.Collections.Generic;
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

			using (var fileProgress = new ConsoleProgressBar())
			{
				encryptionService.OnProgressChanged += (sender, progress) =>
				{
					Console.Title = progress.ToString();
					fileProgress.Report(progress.CurrentFileProgress / 100);
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
					var task = encryptionService.EncryptAsync(sourceFiles, TargetDirectory, TargetType.File,
						cancellationTokenSource.Token);
					task.Wait();
					var result = task.Result;
					OutputResult(result);

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

		private void OutputResult(List<EncryptionResult> result)
		{
			foreach (var encryptionResult in result)
			{
				Console.WriteLine();
				Log(encryptionResult.ToString(), LogLevel.Info);
			}
		}
	}
}
