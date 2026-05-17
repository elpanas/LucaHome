using LucaHome.DTO;
using System.IdentityModel.Tokens.Jwt;

namespace LucaHome.Services
{
    public interface IUserService
    {
        Task<string?> Login(UserDTOIn user);
        // string GenerateJwtToken(string username);
    }
}
