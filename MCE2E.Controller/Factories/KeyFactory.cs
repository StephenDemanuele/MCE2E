using MCE2E.Contracts;
using System.Security.Cryptography;

namespace MCE2E.Controller.Factories
{
	public class KeyFactory : IKeyFactory
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
