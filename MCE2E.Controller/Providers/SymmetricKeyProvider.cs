using System;
using MCE2E.Contracts;
using System.Security.Cryptography;

namespace MCE2E.Controller.Providers
{
	public class SymmetricKeyProvider : ISymmetricKeyProvider
	{
		public byte[] Get(uint keyLength)
		{
			if (keyLength != 16 && keyLength != 24 && keyLength != 32)
			{
				throw new ArgumentException(nameof(keyLength));
			}

			using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				var result = new byte[keyLength];
				rngCryptoServiceProvider.GetBytes(result);

				return result;
			}
		}
	}
}
