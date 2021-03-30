using System.IO;
using MCE2E.Controller.Contracts;

namespace MCE2E.Controller.Providers
{
	internal class FileStreamProvider : IStreamProvider
	{
		public TargetType Target => TargetType.File;

		public Stream Get(string targetFilePath)
		{
			var targetStream = new FileStream(targetFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);

			return targetStream;
		}
	}
}
