using Xunit;
using System;
using System.IO;
using System.Linq;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Bootstrapping;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

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
			var fileToEncrypt = new FileInfo(fileToEncryptPath);

			var privateKeyFilePath = @"C:\Users\Stephen.Demanuele\Desktop\sandbox\privatekey.xml";
			if (!File.Exists(privateKeyFilePath))
			{
				throw new Exception($"File does not exist at {privateKeyFilePath}");
			}
			var privateKeyFile = new FileInfo(privateKeyFilePath);
			
			var targetDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Encrypted"));
			if (Directory.Exists(targetDirectory.FullName))
			{
				Directory.Delete(targetDirectory.FullName, recursive: true);
			}

			var encryptionService = _serviceProvider.GetService<IEncryptionService>();
			encryptionService.EncryptAsync(fileToEncrypt, targetDirectory).Wait();

			var decryptionService = _serviceProvider.GetService<IDecryptionService>();
			var decryptedFiles = decryptionService.Decrypt(targetDirectory, privateKeyFile);

			decryptedFiles.Should().NotBeNullOrEmpty().And.Subject.Count().Should()
			              .Be(1, "because only 1 file was encrypted");

			decryptedFiles[0].Name.Should().BeEquivalentTo(fileToEncrypt.Name);
		}
	}
}
