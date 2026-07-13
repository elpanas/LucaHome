using LucaHome.DTO;
using LucaHome.Factories;
using LucaHome.Models;
using LucaHome.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LucaHome.Services
{
    public class UserService : IUserService
    {
       public readonly IUserRepository _userRepository;
       public UserService(IUserFactory userFactory, IOptions<DBSettings> dbSettings)
        {
            _userRepository = userFactory.GetRepository(dbSettings.Value.DbProvider);
        }

       public async Task<string?> Login(UserDTOIn user) {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                return null;

            User? userExist = await _userRepository.GetUserByUsername(user.Username);

            if (userExist == null) return null;

            bool passwordValid = BCrypt.Net.BCrypt.Verify(user.Password, userExist.Password);

            if (!passwordValid) return null;

            return GenerateJwtToken(user.Username);
        }

       private string GenerateJwtToken(string username)
        {
            // Carica la chiave segreta e il tempo di scadenza dalle variabili d'ambiente
            var key = Environment.GetEnvironmentVariable("JWT_SECRET");
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var expireHours = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS")!);

            // Converte la chiave segreta in un array di byte
            var keyBytes = Encoding.UTF8.GetBytes(key!);

            // Crea i claims per il token (info sull'utente)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Crea le credenziali di firma usando la chiave segreta e l'algoritmo HMAC SHA256
            var creds = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256
                    );

            // Crea il token JWT
            var token = new JwtSecurityToken(
                issuer: issuer, // chi emette il token
                audience: audience, // chi può usare il token
                claims: claims, // informazioni sull'utente
                expires: DateTime.UtcNow.AddHours(expireHours), // scadenza del token
                signingCredentials: creds // credenziali di firma
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

