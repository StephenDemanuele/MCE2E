namespace MCE2E.Contracts
{
	public interface ISymmetricKeyProvider
	{
		byte[] Get(uint keyLength);
	}
}
