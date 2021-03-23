using System;
using MCE2E.Controller.Exceptions;
using MCE2E.Controller.Bootstrapping;

namespace MCE2E.Controller.Services
{
    public class BaseCryptographyService
    {
        protected IServiceProvider _serviceProvider;

        public void Initialize(string pluginLibraryPath = "")
        {
            try
            {
                var serviceProviderBuilder = new ServiceProviderBuilder();
                _serviceProvider = serviceProviderBuilder.Build(pluginLibraryPath);
            }
            catch (Exception ex)
            {
                throw new EncryptionServiceBootstrappingException("Error bootstrapping encryption service. Check inner exception for details", ex);
            }
        }
    }
}
