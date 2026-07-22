using LucaHome.DTO;
using System.IdentityModel.Tokens.Jwt;

namespace LucaHome.Services
{
    public interface IUserService
    {
        Task<bool> Login(UserDTOIn user);
        Task<bool> Handshake(UserDTOIn user);
        // string GenerateJwtToken(string username);
    }
}
