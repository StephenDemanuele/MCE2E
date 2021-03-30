using MCE2E.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.AESEncryption.Bootstrapping
{
    public class DefaultBootstrapper : IBootstrapper
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEncryptionAlgorithm, AESEncryption>();
            serviceCollection.AddSingleton<IDecryptionAlgorithm, AESEncryption>();
        }
    }
}
