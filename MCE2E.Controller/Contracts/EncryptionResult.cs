using System;
using Humanizer;

namespace MCE2E.Controller.Contracts
{
	public class EncryptionResult
	{
		public EncryptionResult(string encryptedFilePath, string keyFilePath, TimeSpan timeTaken)
		{
			EncryptedFilePath = encryptedFilePath;
			KeyFilePath = keyFilePath;
			TimeTaken = timeTaken;
		}

		public string EncryptedFilePath { get; }

		public string KeyFilePath { get; }

		public TimeSpan TimeTaken { get; }

		public override string ToString() => 
			$"File: {EncryptedFilePath}{Environment.NewLine}Key: {KeyFilePath}{Environment.NewLine}{TimeTaken.Humanize()}";
	}
}
