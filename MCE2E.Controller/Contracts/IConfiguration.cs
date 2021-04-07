namespace MCE2E.Controller.Contracts
{
    public interface IConfiguration
    {
        string PathToPublicKey { get; }

        uint SymmetricKeyLength { get; }

        int ChunkSize { get; }

        string PathToOpenSsl { get; }
    }
}
