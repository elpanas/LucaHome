using KyberNET.Api;
using KyberNET.Keys;

namespace LucaHome.Services.Security
{
    public class PqcService : IPqcService
    {
        // Usiamo le classi native della nuova libreria per ML-KEM-768
        private readonly byte[] _serverPublicKey;
        private readonly byte[] _serverPrivateKey;
        private byte[] _currentSharedSecret;

        public PqcService()
        {
            // STEP 1 & 2 & 3: Diventa un'unica riga semplicissima!
            // Genera automaticamente la coppia di chiavi (pubblica e privata) a 768-bit
            // Prima di chiamare GenerateKeyPair
            var keyPair = MlKem768.GenerateKeyPair();
            _serverPublicKey = keyPair.EncapsulationKey.FullBytes;
            _serverPrivateKey = keyPair.DecapsulationKey.FullBytes;
        }

        // Metodo per esportare la chiave pubblica da inviare al client
        public byte[] GetPublicKeyBytes() => _serverPublicKey;

        public void FinalizeHandshake(byte[] clientCiphertext)
        {
            // STEP 4 & 5: Decapsulamento
            // La libreria decifra il ciphertext usando la chiave privata generata all'avvio

            var convertedPrivateKey = KyberDecapsulationKey.FromBytes(_serverPrivateKey);
            var convertedCyphertext = KyberCipherText.FromBytes(clientCiphertext);

            _currentSharedSecret = convertedPrivateKey.Decapsulate(convertedCyphertext);
        }

        public byte[] GetSigningKey() => _currentSharedSecret;
    }
}
