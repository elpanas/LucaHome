using LucaHome.DTO;
using LucaHome.Factories;
using LucaHome.Models;
using LucaHome.Repositories;
using LucaHome.Services.Security;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace LucaHome.Services
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository;
        public readonly IJwtService _jwtService;
        public readonly IPqcService _pqcService;
        public UserService(IUserFactory userFactory,
                           IOptions<DBSettings> dbSettings,
                           IJwtService jwtService,
                           IPqcService pqcService)
        {
            _userRepository = userFactory.GetRepository(dbSettings.Value.DbProvider);
            _jwtService = jwtService;
            _pqcService = pqcService;
        }

        public async Task<bool> Login(UserDTOIn user)
        {
            // 2. Validazione delle credenziali
            User? userExist = await _userRepository.GetUserByUsername(user.Username!);
            if (userExist == null) return false;

            bool passwordValid = BCrypt.Net.BCrypt.Verify(user.Password, userExist.Password);
            if (!passwordValid) return false;

            return true;
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
    }
}

