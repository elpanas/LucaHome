using LucaHome.DTO;
using LucaHome.Services;
using Microsoft.AspNetCore.Mvc;

namespace LucaHome.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // private readonly AuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IUserService userService) { 
            // _authService = authService;
            _userService = userService; 
        }
        /*
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login()
        {
            var check = await _authService.CheckUser(Request);          

            if (check)
                return Ok("Tutto Ok");
            else
                return Unauthorized("Unauthorized");            
        }
        */
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(UserDTOIn user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                return Unauthorized();

            var token = await _userService.Login(user);

            if (token == null) return Unauthorized();

            return Ok(new { Token = token });
        }        
    }
}
