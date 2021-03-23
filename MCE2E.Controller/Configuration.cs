using MCE2E.Controller.Contracts;

namespace MCE2E.Controller
{
    public class Configuration : IConfiguration
    {
        public string PathToPublicKey => @"C:\Users\sdema\Desktop\sandbox\publickey.xml";

        public string PathToOpenSsl => @"C:\Program Files\Git\usr\bin\openssl.exe";
    }
}
