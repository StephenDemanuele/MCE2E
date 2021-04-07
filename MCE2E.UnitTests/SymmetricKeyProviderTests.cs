using Xunit;
using System;
using FluentAssertions;
using MCE2E.Controller.Providers;

namespace MCE2E.UnitTests
{
	public class SymmetricKeyProviderTests
	{
		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(100)]
		public void Get_WhenInvalidKeyLength_ShouldThrowArgumentException(uint keyLength)
		{
			//setup
			var sut = new SymmetricKeyProvider();

			//act
			var exception = Record.Exception(() => sut.Get(keyLength));

			//assert
			exception.Should().BeOfType(typeof(ArgumentException));
		}

		[Theory]
		[InlineData(16)]
		[InlineData(24)]
		[InlineData(32)]
		public void Get_WhenValidKeyLength_ShouldReturnExpectedKeyLength(uint requiredKeyLength)
		{
			//setup
			var sut = new SymmetricKeyProvider();

			//act
			var key = sut.Get(requiredKeyLength);

			//assert
			key.Length.Should().Be((int)requiredKeyLength);
		}
	}
}
