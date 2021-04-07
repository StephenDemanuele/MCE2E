using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MCE2E.Controller.Contracts
{
	public interface IEncryptionService
	{
		Task<List<EncryptionResult>> EncryptAsync(
			FileInfo[] sourceFiles,
			string targetLocation,
			TargetType targetType,
			CancellationToken cancellationToken);

		event OverallProgressReport OnProgressChanged;
	}
}
