using LucaHome.Services;
using Microsoft.AspNetCore.Mvc;

namespace LucaHome.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService) => _authService = authService;

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
    }
}
