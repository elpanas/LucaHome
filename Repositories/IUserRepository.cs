using LucaHome.DTO;

namespace LucaHome.Repositories
{
    public interface IUserRepository
    {
        public Task<bool> Login(UserDTOIn userDTO);
    }
}
