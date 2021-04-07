namespace MCE2E.Controller.Contracts
{
    public interface IConfiguration
    {
        string PathToPublicKey { get; }

        int SymmetricKeyLength { get; }

        int ChunkSize { get; }

        string PathToOpenSsl { get; }
    }
}
