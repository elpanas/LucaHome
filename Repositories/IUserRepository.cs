using LucaHome.DTO;
using LucaHome.Models;

namespace LucaHome.Repositories
{
    public interface IUserRepository
    {
        public Task<User?> GetUserByUsername(string username);
    }
}
