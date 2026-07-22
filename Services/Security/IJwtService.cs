using Microsoft.IdentityModel.Tokens;

namespace LucaHome.Services.Security
{
    public interface IJwtService
    {
        public string GenerateJwtAccessToken(string username);
        public string GenerateRefreshToken(string userId);
    }
}
