namespace MCE2E.Controller.Contracts
{
    public interface IConfiguration
    {
        string PathToPublicKey { get; }

        string PathToOpenSsl { get; }
    }
}
