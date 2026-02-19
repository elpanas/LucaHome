using LucaHome.DTO;

namespace LucaHome.Services
{
    public interface IUserService
    {
        Task<bool> Login(UserDTOIn userDTO);
    }
}
