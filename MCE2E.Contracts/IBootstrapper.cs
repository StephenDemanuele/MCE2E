using Microsoft.Extensions.DependencyInjection;

namespace MCE2E.Contracts
{
    public interface IBootstrapper
    {
        void Register(IServiceCollection serviceCollection);
    }
}
