using MCE2E.Controller.Contracts;

namespace MCE2E.Controller
{
    public class Configuration : IConfiguration
    {
	    public Configuration(string pathToPublicKey, int symmetricKeyLength)
	    {
		    PathToPublicKey = pathToPublicKey;
		    SymmetricKeyLength = symmetricKeyLength;
	    }

	    public string PathToPublicKey { get; }

	    public int SymmetricKeyLength { get; }

	    public string PathToOpenSsl => @"C:\Program Files\Git\usr\bin";
    }
}
