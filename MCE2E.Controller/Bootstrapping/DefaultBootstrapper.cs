using MCE2E.Contracts;
using MCE2E.Controller.Contracts;
using MCE2E.Controller.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.Controller.Bootstrapping
{
    internal class DefaultBootstrapper : IBootstrapper
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConfiguration>(new Configuration());
            serviceCollection.AddSingleton<IKeyFactory, OpenSSLKeyFactory>();
        }
    }
}
