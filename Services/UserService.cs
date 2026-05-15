using LucaHome.DTO;
using LucaHome.Repositories;

namespace LucaHome.Services
{
    public class UserService : IUserService
    {
       public readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> Login(UserDTOIn userDTO) => await _userRepository.Login(userDTO);
    }
}

