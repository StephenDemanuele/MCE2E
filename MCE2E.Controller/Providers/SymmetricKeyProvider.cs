using MCE2E.Contracts;
using System.Security.Cryptography;

namespace MCE2E.Controller.Providers
{
	public class SymmetricKeyProvider : ISymmetricKeyProvider
	{
		public byte[] Get(int keyLength)
		{
			using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				var result = new byte[keyLength];
				rngCryptoServiceProvider.GetBytes(result);

				return result;
			}
		}
	}
}
