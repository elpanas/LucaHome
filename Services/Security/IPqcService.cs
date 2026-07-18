namespace LucaHome.Services.Security
{
    public interface IPqcService
    {
        public byte[] GetPublicKeyBytes();
        public void FinalizeHandshake(byte[] clientCiphertext);
        public byte[]? GetSigningKey();
    }
}
