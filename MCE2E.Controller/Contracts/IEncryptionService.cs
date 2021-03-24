using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MCE2E.Controller.Contracts
{
    public interface IEncryptionService
    {
        Task EncryptAsync(FileInfo publicKeyFile, string targetLocation, CancellationToken cancellationToken);
    }
}
