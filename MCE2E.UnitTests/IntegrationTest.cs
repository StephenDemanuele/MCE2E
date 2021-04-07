using Xunit;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Bootstrapping;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using MCE2E.Controller;

// ReSharper disable PossibleNullReferenceException

namespace MCE2E.UnitTests
{
	public class IntegrationTest
	{
		private readonly IServiceProvider _serviceProvider;
		public IntegrationTest()
		{
			_serviceProvider = new EncryptionServiceProviderBuilder().Build();
		}

		[Fact]
		public void EncryptThenDecryptFileShouldSucceed()
		{
			var fileToEncryptPath = Path.Combine(Environment.CurrentDirectory, "SubjectFiles", "bible.txt");
			if (!File.Exists(fileToEncryptPath))
			{
				throw new Exception($"File does not exist at {fileToEncryptPath}");
			}
			var filesToEncrypt = new FileInfo[] { new FileInfo(fileToEncryptPath) };

			var privateKeyFilePath = @"C:\Users\Stephen.Demanuele\Desktop\sandbox\privatekey.xml";
			if (!File.Exists(privateKeyFilePath))
			{
				throw new Exception($"File does not exist at {privateKeyFilePath}");
			}
			var privateKeyFile = new FileInfo(privateKeyFilePath);

			var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Encrypted");
			var targetDirectory = new DirectoryInfo(targetDirectoryPath);
			if (Directory.Exists(targetDirectory.FullName))
			{
				Directory.Delete(targetDirectory.FullName, recursive: true);
			}

			var encryptionService = _serviceProvider.GetService<IEncryptionService>();
			encryptionService.EncryptAsync(filesToEncrypt, targetDirectoryPath, TargetType.File, CancellationToken.None).Wait();

			var decryptionService = _serviceProvider.GetService<IDecryptionService>();
			var decryptedFiles = decryptionService.Decrypt(targetDirectory, privateKeyFile);

			decryptedFiles.Should().NotBeNullOrEmpty().And.Subject.Count().Should()
						  .Be(1, "because only 1 file was encrypted");

			decryptedFiles[0].Name.Should().BeEquivalentTo(filesToEncrypt[0].Name);
		}
	}
}
