using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MCE2E.Controller;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Providers;
using MCE2E.Controller.Services;
using Moq;
using Xunit;

namespace MCE2E.UnitTests
{
	[Collection("SequentialExecution")]
	public class StreamedEncryptionServiceTests
	{
		private readonly Mock<IConfigurationService> _configurationServiceMock;

		public StreamedEncryptionServiceTests()
		{
			_configurationServiceMock = new Mock<IConfigurationService>();
		}

		[Fact]
		public async Task Encrypt_WhenNullSourceFiles_ShouldThrowArgumentNullException()
		{
			//setup
			var encryptionService = new StreamedEncryptionService(
				_configurationServiceMock.Object, 
				new AESEncryption.AESEncryption(),
				new SymmetricKeyProvider(), 
				new IStreamProvider[] {new FileStreamProvider()});

			var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Encrypted");
			if (Directory.Exists(targetDirectoryPath))
			{
				Directory.Delete(targetDirectoryPath, recursive: true);
			}

			var exception = await Record.ExceptionAsync(() => encryptionService.EncryptAsync(null, targetDirectoryPath, TargetType.File,
				CancellationToken.None));

			exception.Should().BeOfType<ArgumentNullException>();
		}

		[Fact]
		public async Task Encrypt_WhenTargetTypeNotSet_ShouldThrowArgumentException()
		{
			//setup
			var encryptionService = new StreamedEncryptionService(
				_configurationServiceMock.Object,
				new AESEncryption.AESEncryption(),
				new SymmetricKeyProvider(),
				new IStreamProvider[] { new FileStreamProvider() });

			var fileToEncryptPath = Path.Combine(Environment.CurrentDirectory, "SubjectFiles", "bible.txt");
			if (!File.Exists(fileToEncryptPath))
			{
				throw new Exception($"File does not exist at {fileToEncryptPath}");
			}
			var filesToEncrypt = new FileInfo[] { new FileInfo(fileToEncryptPath) };

			var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Encrypted");
			if (Directory.Exists(targetDirectoryPath))
			{
				Directory.Delete(targetDirectoryPath, recursive: true);
			}

			var exception = await Record.ExceptionAsync(() => 
				encryptionService.EncryptAsync(filesToEncrypt, targetDirectoryPath, TargetType.NotSet,
				CancellationToken.None));

			exception.Should().BeOfType<ArgumentException>();
		}

		[Fact]
		public async Task Encrypt_WhenStreamProviderForTargetTypeNotAvailable_ShouldThrowInvalidOperationException()
		{
			//setup
			var encryptionService = new StreamedEncryptionService(
				_configurationServiceMock.Object,
				new AESEncryption.AESEncryption(),
				new SymmetricKeyProvider(),
				new IStreamProvider[] { new FileStreamProvider() });

			var fileToEncryptPath = Path.Combine(Environment.CurrentDirectory, "SubjectFiles", "bible.txt");
			if (!File.Exists(fileToEncryptPath))
			{
				throw new Exception($"File does not exist at {fileToEncryptPath}");
			}
			var filesToEncrypt = new FileInfo[] { new FileInfo(fileToEncryptPath) };

			var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Encrypted");
			if (Directory.Exists(targetDirectoryPath))
			{
				Directory.Delete(targetDirectoryPath, recursive: true);
			}

			var exception = await Record.ExceptionAsync(() =>
				encryptionService.EncryptAsync(filesToEncrypt, targetDirectoryPath, TargetType.Ftp, CancellationToken.None));

			exception.Should().BeOfType<InvalidOperationException>();
		}

		[Fact]
		public async Task Encrypt_WhenTargetLocationNull_ShouldThrowArgumentNullException()
		{
			//setup
			var encryptionService = new StreamedEncryptionService(
				_configurationServiceMock.Object,
				new AESEncryption.AESEncryption(),
				new SymmetricKeyProvider(),
				new IStreamProvider[] { new FileStreamProvider() });

			var fileToEncryptPath = Path.Combine(Environment.CurrentDirectory, "SubjectFiles", "bible.txt");
			if (!File.Exists(fileToEncryptPath))
			{
				throw new Exception($"File does not exist at {fileToEncryptPath}");
			}
			var filesToEncrypt = new FileInfo[] { new FileInfo(fileToEncryptPath) };

			var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Encrypted");
			if (Directory.Exists(targetDirectoryPath))
			{
				Directory.Delete(targetDirectoryPath, recursive: true);
			}

			var exception = await Record.ExceptionAsync(() =>
				encryptionService.EncryptAsync(filesToEncrypt, null, TargetType.File, CancellationToken.None));

			exception.Should().BeOfType<ArgumentNullException>();
		}

		[Fact]
		public async Task Encrypt_WhenTargetLocationInvalid_ShouldThrowArgumentException()
		{
			//setup
			var encryptionService = new StreamedEncryptionService(
				_configurationServiceMock.Object,
				new AESEncryption.AESEncryption(),
				new SymmetricKeyProvider(),
				new IStreamProvider[] { new FileStreamProvider() });

			var fileToEncryptPath = Path.Combine(Environment.CurrentDirectory, "SubjectFiles", "bible.txt");
			if (!File.Exists(fileToEncryptPath))
			{
				throw new Exception($"File does not exist at {fileToEncryptPath}");
			}
			var filesToEncrypt = new FileInfo[] { new FileInfo(fileToEncryptPath) };

			var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Encrypted");
			if (Directory.Exists(targetDirectoryPath))
			{
				Directory.Delete(targetDirectoryPath, recursive: true);
			}

			var targetLocation = "foo bar";
			var exception = await Record.ExceptionAsync(() =>
				encryptionService.EncryptAsync(filesToEncrypt, targetLocation, TargetType.File, CancellationToken.None));

			exception.Should().BeOfType<ArgumentException>();
		}

		[Fact]
		public async Task Encrypt_WhenEmptySourceFiles_ShouldSucceed()
		{
			//setup
			var encryptionService = new StreamedEncryptionService(
				_configurationServiceMock.Object,
				new AESEncryption.AESEncryption(),
				new SymmetricKeyProvider(),
				new IStreamProvider[] { new FileStreamProvider() });

			var targetDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Encrypted");
			if (Directory.Exists(targetDirectoryPath))
			{
				Directory.Delete(targetDirectoryPath, recursive: true);
			}

			var result = await encryptionService.EncryptAsync(new FileInfo[0], targetDirectoryPath, TargetType.File,
				CancellationToken.None);

			result.Should().BeEmpty();
		}
	}
}
