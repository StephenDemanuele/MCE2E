using System;
using System.IO;
using System.Linq;
using MCE2E.Controller.Contracts;

namespace MCE2E.Controller
{
	public class Configuration : IConfiguration
	{
		private readonly int[] _validKeyLengths = new int[] { 16, 24, 32 };

		public Configuration(string pathToPublicKey, int symmetricKeyLength)
		{
			if (!_validKeyLengths.Contains(symmetricKeyLength))
			{
				throw new ArgumentException(
					$"Invalid value ({symmetricKeyLength}) for {nameof(symmetricKeyLength)}. Must be either of 16, 24, 32");
			}
			if (!File.Exists(pathToPublicKey))
			{
				throw new ArgumentException(
					$"Invalid value ({pathToPublicKey}) for {nameof(pathToPublicKey)}, file does not exist");
			}

			PathToPublicKey = pathToPublicKey;
			SymmetricKeyLength = symmetricKeyLength;
		}

		public string PathToPublicKey { get; }

		/// <summary>
		/// Valid values are 16, 24, 32
		/// </summary>
		public int SymmetricKeyLength { get; }

		public int ChunkSize { get; } = 1 << 10;

		public string PathToOpenSsl => @"C:\Program Files\Git\usr\bin";
	}
}
