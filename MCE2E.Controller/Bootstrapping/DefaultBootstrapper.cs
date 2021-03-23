using MCE2E.Contracts;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Factories;
using MCE2E.Controller.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.Controller.Bootstrapping
{
	internal class DefaultBootstrapper : IBootstrapper
	{
		public void Register(IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<IConfigurationService, DefaultConfigurationService>();
			serviceCollection.AddSingleton<IKeyFactory, KeyFactory>();
			serviceCollection.AddSingleton<IEncryptionService, DefaultEncryptionService>();
			serviceCollection.AddSingleton<IDecryptionService, DefaultDecryptionService>();
		}
	}
}
