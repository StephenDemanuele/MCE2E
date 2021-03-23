using Xunit;
using System;
using System.IO;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Bootstrapping;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable PossibleNullReferenceException

namespace MCE2E.UnitTests
{
	public class UnitTest1
	{
		private readonly IServiceProvider _serviceProvider;
		public UnitTest1()
		{
			_serviceProvider = new EncryptionServiceProviderBuilder().Build();
		}

		[Fact]
		public void Test1()
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
				Directory.Delete(targetDirectory.FullName);
			}

			var encryptionService = _serviceProvider.GetService<IEncryptionService>();
			encryptionService.EncryptAsync(fileToEncrypt, targetDirectory).Wait();

			var decryptionService = _serviceProvider.GetService<IDecryptionService>();
			var decryptedFiles= decryptionService.Decrypt(targetDirectory, privateKeyFile);

		}
	}
}
