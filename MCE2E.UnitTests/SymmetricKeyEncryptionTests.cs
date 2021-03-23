using Xunit;
using System;
using System.IO;
using FluentAssertions;
using MCE2E.DefaultEncryption;
using MCE2E.Controller.Factories;
	
namespace MCE2E.UnitTests
{
	public class SymmetricKeyEncryptionTests
	{
		private readonly string _privateKeyFilePath;
		private readonly string _publicKeyFilePath;

		public SymmetricKeyEncryptionTests()
		{
			_privateKeyFilePath= Path.Combine(Environment.CurrentDirectory, "pub-priv-key-pair", "privatekey.xml");
			_publicKeyFilePath = Path.Combine(Environment.CurrentDirectory, "pub-priv-key-pair", "publickey.xml");
		}

		[Fact]
		public void DecryptingEncryptedKeyShouldBeSameAsOriginalKey()
		{
			//setup
			var keyFactory = new KeyFactory();
			var symmetricKey = keyFactory.Get(16);
			var sut = new AESEncryption();

			//act
			var encryptedSymmetricKey = sut.EncryptSymmetricKey(symmetricKey, _publicKeyFilePath);
			var decryptedSymmetricKey = sut.DecryptSymmetricKey(encryptedSymmetricKey, _privateKeyFilePath);

			//assert
			encryptedSymmetricKey.Should().NotBeEquivalentTo(symmetricKey, "encryption should return a different key");
			decryptedSymmetricKey.Should().BeEquivalentTo(symmetricKey, "decryption reverses encryption");
		}

		[Fact]
		public void EncryptionAttemptWithNullKeyShouldThrowArgumentNullException()
		{
			//setup
			var sut = new AESEncryption();

			//act
			var exception = Record.Exception(() => sut.EncryptSymmetricKey(null, pathToPublicKey: null));
			exception.Should().BeOfType(typeof(ArgumentNullException));
		}

		[Fact]
		public void EncryptionAttemptWithNullPublicKeyPathShouldThrowArgumentNullException()
		{
			//setup
			var keyFactory = new KeyFactory();
			var symmetricKey = keyFactory.Get(16);
			var sut = new AESEncryption();

			//act
			var exception = Record.Exception(() => sut.EncryptSymmetricKey(symmetricKey, pathToPublicKey: null));
			exception.Should().BeOfType(typeof(ArgumentNullException));
		}

		[Fact]
		public void EncryptionAttemptWithMissingPublicKeyPathShouldThrowFileNotFoundException()
		{
			//setup
			var keyFactory = new KeyFactory();
			var symmetricKey = keyFactory.Get(16);
			var sut = new AESEncryption();

			//act
			var exception = Record.Exception(() => sut.EncryptSymmetricKey(symmetricKey, pathToPublicKey: "meh"));
			exception.Should().BeOfType(typeof(FileNotFoundException));
		}

		[Fact]
		public void DecryptionAttemptWithNullKeyShouldThrowArgumentNullException()
		{
			//setup
			var sut = new AESEncryption();

			//act
			var exception = Record.Exception(() => sut.DecryptSymmetricKey(null, privateKeyFilePath: null));
			exception.Should().BeOfType(typeof(ArgumentNullException));
		}

		[Fact]
		public void DecryptionAttemptWithNullPrivateKeyPathShouldThrowArgumentNullException()
		{
			//setup
			var keyFactory = new KeyFactory();
			var symmetricKey = keyFactory.Get(16);
			var sut = new AESEncryption();

			//act
			var exception = Record.Exception(() => sut.DecryptSymmetricKey(null, privateKeyFilePath: null));
			exception.Should().BeOfType(typeof(ArgumentNullException));
		}

		[Fact]
		public void DecryptionAttemptWithMissingPrivateKeyPathShouldThrowFileNotFoundException()
		{
			//setup
			var keyFactory = new KeyFactory();
			var symmetricKey = keyFactory.Get(16);
			var sut = new AESEncryption();

			//act
			var exception = Record.Exception(() => sut.DecryptSymmetricKey(symmetricKey, privateKeyFilePath: "meh"));
			exception.Should().BeOfType(typeof(FileNotFoundException));
		}
	}
}
