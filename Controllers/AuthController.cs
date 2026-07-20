using LucaHome.DTO;
using LucaHome.Services;
using LucaHome.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;

namespace LucaHome.Controllers
{
    [Route("api/loginpqc")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPqcService _pqcService;

        public AuthController(IUserService userService, IPqcService pqcService)
        {
            _userService = userService;
            _pqcService = pqcService;
        }

        // 1. GET api/login/handshake -> Astro recupera la chiave pubblica del server
        [HttpGet("handshake")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Handshake()
        {
            // Ottiene la chiave pubblica dal tuo servizio
            var publicKeyBytes = _pqcService.GetPublicKeyBytes();

            // Converte in Base64 per spedirla via JSON ad Astro
            string publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);

            return Ok(new
            {
                // la chiave privata è fissa in memoria sul PqcService (registrato come Singleton)
                PublicKeyBase64 = publicKeyBase64
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login(UserDTOIn user)
        {
            // Controllo formale dell'input (incluso il nuovo campo Signature)
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) ||
                string.IsNullOrEmpty(user.Ciphertext) || string.IsNullOrEmpty(user.Signature))
            {
                return Unauthorized();
            }

            // Completa l'handshake
            var handshakeResult = await _userService.Handshake(user);
            if (!handshakeResult) return Unauthorized(new { error = "Handshake fallito" });

            // Effettua il login classico e riceve il token
            var token = await _userService.Login(user);
            if (token == null) return Unauthorized(new { error = "Credenziali errate o inesistenti" });

            var expireHours = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS")!);

            // Aggiungo il cookie con il token
            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true, // JS non può leggere il cookie
                Secure = true, // il browser può inviarlo solo tramite HTTPS
                SameSite = SameSiteMode.None, // invia il cookie solo se la req proviene dallo stesso dominio che l'ha impostato
                Expires = DateTime.UtcNow.AddHours(expireHours) // scadenza del cookie
            });

            return Ok(new { message = "Login effettuato con successo" });
        }

        [Authorize]
        [HttpGet("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> RefreshToken()
        {
            // Recuperi l'utente dal contesto
            var username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (username == null) return Unauthorized();

            // Generi il nuovo token
            string newToken = _userService.GenerateJwtToken(username);

            // Sovrascrivi il cookie
            Response.Cookies.Append("AuthToken", newToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(2)
            });

            return Ok();
        }
    }
}
