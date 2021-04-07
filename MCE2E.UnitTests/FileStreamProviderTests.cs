using Xunit;
using System;
using System.IO;
using FluentAssertions;
using MCE2E.Controller.Providers;

namespace MCE2E.UnitTests
{
	public class FileStreamProviderTests
	{
		[Fact]
		public void Get_WhenValidFilePath_ShouldReturnAValidStream()
		{
			//setup
			var sut = new FileStreamProvider();
			var targetFilePath = Path.Combine(Environment.CurrentDirectory, "testfile1.txt");
			
			//act
			var targetStream = sut.Get(targetFilePath);

			//assert
			Assert.NotNull(targetStream);

			//cleanup
			targetStream.Close();
			File.Delete(targetFilePath);
		}

		[Fact]
		public void Get_WhenValidFilePathAndFileAlreadyExists_ShouldReturnAValidStream()
		{
			//setup
			var targetFilePath = Path.Combine(Environment.CurrentDirectory, "testfile1.txt");
			File.WriteAllText(targetFilePath, "should be overwritten");

			var sut = new FileStreamProvider();

			//act
			var targetStream = sut.Get(targetFilePath);

			//assert
			Assert.NotNull(targetStream);

			//cleanup
			targetStream.Close();
			File.Delete(targetFilePath);
		}

		[Fact]
		public void Get_WhenNullFilePath_ShouldThrowArgumentNullException()
		{
			//setup
			var fileStreamProvider = new FileStreamProvider();

			//act
			var exception = Record.Exception( () =>fileStreamProvider.Get(null));
			
			//assert
			exception.Should().BeOfType(typeof(ArgumentNullException));
		}

		[Fact]
		public void ReadFromStream__ShouldNotBeAllowed()
		{
			//setup
			var targetFilePath = Path.Combine(Environment.CurrentDirectory, "testfile1.txt");
			File.WriteAllText(targetFilePath, "should be overwritten");

			var sut = new FileStreamProvider();

			//act
			var targetStream = sut.Get(targetFilePath);


			//assert
			Assert.NotNull(targetStream);
			Assert.False(targetStream.CanRead);

			//cleanup
			targetStream.Close();
			File.Delete(targetFilePath);
		}

		[Fact]
		public void Cleaup_WhenNullFilePath_ShouldThrowArgumentNullException()
		{
			//setup
			var fileStreamProvider = new FileStreamProvider();

			//act
			var exception = Record.Exception(() => fileStreamProvider.CleanUp(null));

			//assert
			exception.Should().BeOfType(typeof(ArgumentNullException));
		}

		[Fact]
		public void Cleanup_WhenFileExists_ShouldDeleteFile()
		{
			//setup
			var targetFilePath = Path.Combine(Environment.CurrentDirectory, "testfile1.txt");
			File.WriteAllText(targetFilePath, "will be deleted");
			var sut = new FileStreamProvider();

			//act
			sut.CleanUp(targetFilePath);

			//assert
			Assert.False(File.Exists(targetFilePath));
		}

		[Fact]
		public void Cleanup_WhenFileDoesNotExist_ShouldNotThrow()
		{
			//setup
			var targetFilePath = Path.Combine(Environment.CurrentDirectory, "testfile1.txt");
			var sut = new FileStreamProvider();

			//act
			sut.CleanUp(targetFilePath);

			//assert
			Assert.False(File.Exists(targetFilePath));
		}
	}
}
