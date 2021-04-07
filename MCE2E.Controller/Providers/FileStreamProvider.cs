using System;
using System.IO;
using MCE2E.Controller.Contracts;

namespace MCE2E.Controller.Providers
{
	internal class FileStreamProvider : IStreamProvider
	{
		public TargetType Target => TargetType.File;

		public Stream Get(string targetFilePath)
		{
			if (string.IsNullOrEmpty(targetFilePath))
			{
				throw new ArgumentNullException(nameof(targetFilePath));
			}

			var targetStream = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

			return targetStream;
		}

		public void CleanUp(string targetFilePath)
		{ 
			if (string.IsNullOrEmpty(targetFilePath))
			{
				throw new ArgumentNullException(nameof(targetFilePath));
			}

			if (File.Exists(targetFilePath))
				File.Delete(targetFilePath);
		}
	}
}
