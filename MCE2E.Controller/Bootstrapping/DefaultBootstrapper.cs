using MCE2E.Contracts;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Providers;
using MCE2E.Controller.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.Controller.Bootstrapping
{
	internal class DefaultBootstrapper : IBootstrapper
	{
		public void Register(IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<IConfigurationService, DefaultConfigurationService>();
			serviceCollection.AddSingleton<ISymmetricKeyProvider, SymmetricKeyProvider>();
			serviceCollection.AddSingleton<IEncryptionService, StreamedEncryptionService>();
			serviceCollection.AddSingleton<IDecryptionService, DefaultDecryptionService>();
			serviceCollection.AddSingleton<IStreamProvider, FileStreamProvider>();
			serviceCollection.AddSingleton<IStreamProvider, FtpStreamProvider>();
		}
	}
}
