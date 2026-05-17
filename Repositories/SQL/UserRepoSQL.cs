using LucaHome.DBs;
using LucaHome.DTO;
using LucaHome.Models;

namespace LucaHome.Repositories.SQL
{
    public class UserRepoSQL : IUserRepository
    {
        private readonly SQLDBContext _context;
        public UserRepoSQL(SQLDBContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserByUsername(string username) => 
            _context.Users.FirstOrDefault(u => u.Username == username);
    }
}
