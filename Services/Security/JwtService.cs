using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LucaHome.Services.Security
{
    public class JwtService : IJwtService
    {
        private readonly IMemoryCache _cache;
        private static SymmetricSecurityKey _currentJwtSecret;
        private static FileSystemWatcher _watcher;
        private static readonly string _filePath = "/app/keys/jwt_secret.bin";

        static JwtService()
        {
            // Appena istanzia la classe con il builder in Program.cs...

            if (File.Exists(_filePath))
            {
                // 1. Carica la chiave iniziale in RAM all'avvio
                _currentJwtSecret = BytesToKey();

                // 2. Configura il Watcher per aggiornare la RAM solo quando il file cambia sul disco
                _watcher = new FileSystemWatcher(Path.GetDirectoryName(_filePath)!)
                {
                    Filter = Path.GetFileName(_filePath),
                    NotifyFilter = NotifyFilters.LastWrite
                };

                _watcher.Changed += (sender, e) =>
                {
                    // Il watcher esegue questo codice ogni 24h, quando cambia il file su disco
                    _currentJwtSecret = BytesToKey();
                };

                _watcher.EnableRaisingEvents = true;
            }
            else
            {
                var jwtBytes = GenerateJwtSecret();
                _currentJwtSecret = new SymmetricSecurityKey(jwtBytes);
            }
        }

        public JwtService(IMemoryCache cache)
        {
            _cache = cache;
        }

        private static SymmetricSecurityKey BytesToKey() => new(File.ReadAllBytes(_filePath));


        // Restituisce il JWT aggiornato
        public static IEnumerable<SecurityKey> CurrentJwtSecret(string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters)
        {
            return [_currentJwtSecret];
        }

        public string GenerateJwtAccessToken(string username)
        {
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var expireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_MINUTES")!);

            byte[] keyBytes = _currentJwtSecret.Key;

            // Crea la firma del server con il jwt_secret usando l'algoritmo Hmac ed SHA256
            // Come a dire: "io, il server, ho creato questo token a partire dal JWT_SECRET che conosco solo io
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256
            );

            // Crea un oggetto con dati specifici dell'utente
            var claims = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ]);

            // Qui mette insieme tutte le informaioni
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience, // gruppo di destinatari
                Subject = claims, // utente a cui è riferito il token
                Expires = TimeProvider.System.GetUtcNow().AddHours(expireMinutes).UtcDateTime,
                SigningCredentials = creds // firma del server
            };

            // Crea finalmente il token in base ai dati forniti
            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }

        public string GenerateRefreshToken(string username)
        {
            var randomNumber = new byte[64]; // numero random di 64 byte
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber); // converte in byte
            string refreshToken = Convert.ToBase64String(randomNumber);

            var expireDays = int.Parse(Environment.GetEnvironmentVariable("REFRESH_EXPIRE_DAYS")!);

            // 3. Imposta le opzioni di scadenza per la cache nativa
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(expireDays)
            };

            // 4. SALVA NELLA CACHE: Chiave = RefreshToken, Valore = UserId
            _cache.Set($"refresh_token:{refreshToken}", username, cacheOptions);

            return refreshToken;
        }

        private static byte[] GenerateJwtSecret()
        {
            int sizeInBytes = 32; // 256 bits
            // Crea un array vuoto di byte della dimensione specificata
            byte[] randomBytes = new byte[sizeInBytes];

            // Riempi l'array con byte casuali sicuri
            RandomNumberGenerator.Fill(randomBytes);

            return randomBytes;
        }
    }
}
