using Xunit;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Bootstrapping;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using MCE2E.Controller;

// ReSharper disable PossibleNullReferenceException

namespace MCE2E.UnitTests
{
	[Collection("SequentialExecution")]
	public class IntegrationTest
	{
		private readonly IServiceProvider _serviceProvider;
		public IntegrationTest()
		{
			_serviceProvider = new EncryptionServiceProviderBuilder().Build();
		}

		[Fact]
		public async Task EncryptThenDecryptFileShouldSucceed()
		{
			var fileToEncryptPath1 = Path.Combine(Environment.CurrentDirectory, "SubjectFiles", "bible.txt");
			if (!File.Exists(fileToEncryptPath1))
			{
				throw new Exception($"File does not exist at {fileToEncryptPath1}");
			}
			var fileToEncryptPath2 = Path.Combine(Environment.CurrentDirectory, "SubjectFiles", "bible2.txt");
			if (!File.Exists(fileToEncryptPath2))
			{
				throw new Exception($"File does not exist at {fileToEncryptPath2}");
			}
			var filesToEncrypt = new FileInfo[] { new FileInfo(fileToEncryptPath1), new FileInfo(fileToEncryptPath2) };

			var privateKeyFilePath = @"C:\Users\Stephen.Demanuele\Desktop\sandbox\privatekey.xml";
			if (!File.Exists(privateKeyFilePath))
			{
				throw new Exception($"File does not exist at {privateKeyFilePath}");
			}

			var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Encrypted");
			var targetDirectory = new DirectoryInfo(targetDirectoryPath);
			if (Directory.Exists(targetDirectory.FullName))
			{
				Directory.Delete(targetDirectory.FullName, recursive: true);
			}

			var encryptionService = _serviceProvider.GetService<IEncryptionService>();
			var result = await encryptionService.EncryptAsync(filesToEncrypt, targetDirectoryPath, TargetType.File, CancellationToken.None);

			var decryptionService = _serviceProvider.GetService<IDecryptionService>();
			var decryptedFiles = decryptionService.Decrypt(targetDirectory, new FileInfo(privateKeyFilePath));

			decryptedFiles.Should().NotBeNullOrEmpty().And.Subject.Count().Should()
						  .Be(filesToEncrypt.Length, "because all files should be encrypted");

			for (var i = 0; i < decryptedFiles.Count; i++)
			{
				var decryptedFile = decryptedFiles[i];
				decryptedFile.Name.Should().BeEquivalentTo(filesToEncrypt[i].Name);
				decryptedFile.Length.Should().Be(filesToEncrypt[i].Length);
			}
		}
	}
}
