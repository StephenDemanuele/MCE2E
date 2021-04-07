using System.IO;

namespace MCE2E.Controller.Contracts
{
	public interface IStreamProvider
	{
		TargetType Target { get; }

		Stream Get(string targetFilePath);

		void CleanUp(string targetFilePath);
	}
}
