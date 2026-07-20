using LucaHome.DTO;
using LucaHome.Factories;
using LucaHome.Models;
using LucaHome.Repositories;
using LucaHome.Services.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LucaHome.Services
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly IJwtSecretService _jwtGeneratorService;
        public readonly IPqcService _pqcService;
        public UserService(IUserFactory userFactory,
                           IOptions<DBSettings> dbSettings,
                           IJwtSecretService jwtGeneratorService,
                           IPqcService pqcService)
        {
            _userRepository = userFactory.GetRepository(dbSettings.Value.DbProvider);
            _jwtGeneratorService = jwtGeneratorService;
            _pqcService = pqcService;
        }

        public async Task<string?> Login(UserDTOIn user)
        {
            // 2. Validazione delle credenziali
            User? userExist = await _userRepository.GetUserByUsername(user.Username!);
            if (userExist == null) return null;

            bool passwordValid = BCrypt.Net.BCrypt.Verify(user.Password, userExist.Password);
            if (!passwordValid) return null;

            // 3. Rilascio del token di autorizzazione
            return GenerateJwtToken(user.Username!);
        }

        public async Task<bool> Handshake(UserDTOIn user)
        {
            // 1. Validazione crittografica (Handshake Post-Quantistico)
            try
            {
                // Decodifica i dati ricevuti dal client
                byte[] ciphertextBytes = Convert.FromBase64String(user.Ciphertext!);
                byte[] clientSignature = Convert.FromBase64String(user.Signature!);

                // Decapsulazione locale
                byte[] localSharedSecret = _pqcService.FinalizeHandshake(ciphertextBytes);

                // Calcolo e verifica HMAC
                byte[] expectedSignature = _pqcService.GetSignatureBytes(user.Username + user.Password, localSharedSecret);

                // Ripulisce l'array per sicurezza
                Array.Clear(localSharedSecret);

                // Firma non corrispondente (Kyber fallito o manipolato)
                if (!CryptographicOperations.FixedTimeEquals(expectedSignature, clientSignature)) return false;

                return true;
            }
            catch (Exception)
            {
                return false; // Errore di parsing o decapsulazione
            }
        }

        public string GenerateJwtToken(string username)
        {
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var expireHours = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS")!);

            byte[] keyBytes = _jwtGeneratorService.TakeJwtSecretFromFile();

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
                Expires = TimeProvider.System.GetUtcNow().AddHours(expireHours).UtcDateTime,
                SigningCredentials = creds // firma del server
            };

            // Crea finalmente il token in base ai dati forniti
            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }
    }
}

