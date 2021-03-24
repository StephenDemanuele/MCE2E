using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MCE2E.Controller.Contracts
{
	public interface IStreamedEncryptionService
	{
		Task EncryptAsync(FileInfo sourceFileToEncrypt, string targetLocation,  CancellationToken cancellationToken);
	}
}
