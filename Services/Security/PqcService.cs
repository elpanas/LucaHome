using KyberNET.Api;
using KyberNET.Keys;
using System.Security.Cryptography;

namespace LucaHome.Services.Security
{
    public class PqcService : IPqcService
    {
        // Chiavi pubblica, privata e shared secret dell'utente
        private readonly byte[] _serverPublicKey;
        private readonly byte[] _serverPrivateKey;
        //private byte[]? _currentSharedSecret;

        public PqcService()
        {
            // Genera automaticamente la coppia di chiavi (pubblica e privata) a 768-bit
            var keyPair = MlKem768.GenerateKeyPair();
            _serverPublicKey = keyPair.EncapsulationKey.FullBytes;
            _serverPrivateKey = keyPair.DecapsulationKey.FullBytes;
        }

        // Metodo per esportare la chiave pubblica da inviare al client
        public byte[] GetPublicKeyBytes() => _serverPublicKey;

        // Metodo per decapsulare il ciphertext ricevuto dal client e ottenere il shared secret
        public byte[] FinalizeHandshake(byte[] clientCiphertext)
        {
            // La libreria decifra il ciphertext usando la chiave privata generata all'avvio
            var convertedPrivateKey = KyberDecapsulationKey.FromBytes(_serverPrivateKey);
            var convertedCyphertext = KyberCipherText.FromBytes(clientCiphertext);

            return convertedPrivateKey.Decapsulate(convertedCyphertext); // Restituisce il shared secret
        }

        public byte[] GetSignatureBytes(string credentials, byte[] sharedSecret)
        {
            var hmac = new HMACSHA256(sharedSecret);

            byte[] credentialsBytes = System.Text.Encoding.UTF8.GetBytes(credentials);
            return hmac.ComputeHash(credentialsBytes);

        }

        //public byte[]? GetSigningKey() => _currentSharedSecret;
    }
}
