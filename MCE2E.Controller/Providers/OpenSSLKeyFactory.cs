using System;
using System.IO;
using System.Linq;
using MCE2E.Contracts;
using System.Diagnostics;
using MCE2E.Controller.Contracts;

namespace MCE2E.Controller.Factories
{
	internal class OpenSSLKeyFactory : ISymmetricKeyProvider
	{
		private readonly IConfiguration _configuration;

		public OpenSSLKeyFactory(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public byte[] Get(int keyLength)
		{
			if (!File.Exists(_configuration.PathToOpenSsl))
			{
				throw new FileNotFoundException("OpenSSL is missing", _configuration.PathToOpenSsl);
			}

			var processStartInfo = new ProcessStartInfo
			{
				FileName = _configuration.PathToOpenSsl,
				Arguments = $"rand -hex {keyLength}",
				RedirectStandardOutput = true,
				UseShellExecute = false
			};

			var process = new Process();
			process.StartInfo = processStartInfo;

			process.Start();
			var key = process.StandardOutput.ReadToEnd().Trim();

			return HexStringToByteArray(key);
		}

		private byte[] HexStringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
							 .Where(x => x % 2 == 0)
							 .Select(x => Convert.ToByte(hex.Substring(x, 2), fromBase: 16))
							 .ToArray();
		}
	}
}
