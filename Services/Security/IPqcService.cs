namespace LucaHome.Services.Security
{
    public interface IPqcService
    {
        public byte[] GetPublicKeyBytes();
        public byte[] FinalizeHandshake(byte[] clientCiphertext);
        public byte[] GetSignatureBytes(string credentials, byte[] sharedSecret);

        //public byte[]? GetSigningKey();
    }
}
