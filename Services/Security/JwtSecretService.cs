using System.Security.Cryptography;

namespace LucaHome.Services.Security
{
    public class JwtSecretService : IJwtSecretService
    {
        private static readonly byte[] _localJwtSecret = GenerateJwtSecret();
        private static byte[] GenerateJwtSecret()
        {
            int sizeInBytes = 32; // 256 bits
            // Crea un array vuoto di byte della dimensione specificata
            byte[] randomBytes = new byte[sizeInBytes];

            // Riempi l'array con byte casuali sicuri
            RandomNumberGenerator.Fill(randomBytes);

            return randomBytes;
        }

        public byte[] TakeJwtSecretFromFile()
        {
            const string filePath = "/app/keys/jwt_secret.bin";

            return (!File.Exists(filePath)) ? _localJwtSecret : File.ReadAllBytes(filePath);           
        }
    }
}
