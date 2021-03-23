using MCE2E.Controller.Contracts;

namespace MCE2E.Controller.Services
{
	internal class DefaultConfigurationService : IConfigurationService
	{
		public IConfiguration Get()
		{
			return new Configuration(@"C:\Users\Stephen.Demanuele\Desktop\sandbox\publickey.xml", 16);
		}
	}
}
