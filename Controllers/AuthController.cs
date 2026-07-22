using Azure.Core;
using LucaHome.DTO;
using LucaHome.Services;
using LucaHome.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;

namespace LucaHome.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPqcService _pqcService;
        private readonly IJwtService _jwtService;
        private readonly IMemoryCache _cache;

        public AuthController(IUserService userService, IPqcService pqcService, IJwtService jwtService, IMemoryCache cache)
        {
            _userService = userService;
            _pqcService = pqcService;
            _jwtService = jwtService;
            _cache = cache;
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

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login([FromBody] UserDTOIn user)
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

            // Effettua il login classico
            var userExists = await _userService.Login(user);
            if (!userExists) return Unauthorized(new { error = "Credenziali errate o inesistenti" });

            // Genera l'accesso token (JWT)
            var accessToken = _jwtService.GenerateJwtAccessToken(user.Username);

            // Genera il Refresh Token
            var refreshToken = _jwtService.GenerateRefreshToken(user.Username);

            // Recupera le durate dei token
            var expireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_MINUTES")!);
            var expireDays = int.Parse(Environment.GetEnvironmentVariable("REFRESH_EXPIRE_DAYS")!);

            // Aggiungo il cookie con il token
            Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
            {
                HttpOnly = true, // JS non può leggere il cookie
                Secure = true, // il browser può inviarlo solo tramite HTTPS
                SameSite = SameSiteMode.None, // accetta cookie provenienti anche da domini diversi
                Expires = DateTimeOffset.UtcNow.AddHours(expireMinutes) // scadenza del cookie
            });

            Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true, // JS non può leggere il cookie
                Secure = true, // il browser può inviarlo solo tramite HTTPS
                SameSite = SameSiteMode.None, // accetta cookie provenienti anche da domini diversi
                Path = "/api/auth/refresh",
                Expires = DateTimeOffset.UtcNow.AddDays(expireDays) // scadenza del cookie
            });

            return Ok(new { Smessage = "Login effettuato con successo" });
        }

        [Authorize]
        [HttpGet("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("ApiPolicy")]
        public async Task<IActionResult> RefreshToken()
        {
            // 1. Recupera il vecchio Refresh Token dal cookie
            if (!Request.Cookies.TryGetValue("RefreshToken", out var oldRefreshToken))
                return Unauthorized("Nessun Refresh Token.");

            // 2. Cerca nella IMemoryCache nativa di .NET
            if (!_cache.TryGetValue($"refresh_token:{oldRefreshToken}", out string username))            
                return Unauthorized(new { error = "Sessione scaduta o non valida" });

            // Genera il nuovo Access Token JWT
            var newAccessToken = _jwtService.GenerateJwtAccessToken(username);
            var expireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_MINUTES")!);

            Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
            {
                HttpOnly = true, // JS non può leggere il cookie
                Secure = true, // il browser può inviarlo solo tramite HTTPS
                SameSite = SameSiteMode.None, // accetta cookie provenienti anche da domini diversi
                Expires = DateTimeOffset.UtcNow.AddHours(expireMinutes) // scadenza del cookie
            });

            return Ok();
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            // 1. Legge il RefreshToken dal cookie per sapere quale sessione cancellare
            if (Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))   
                _cache.Remove($"refresh_token:{refreshToken}");            

            // 3. Inganno il browser inviandogli cookie scaduti nel 1970, che lui elimina ovviamente
            var accessOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UnixEpoch
            };

            var refreshOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/auth/refresh",
                Expires = DateTimeOffset.UnixEpoch
            };

            // Invia un cookie (contenitore) vuoto
            Response.Cookies.Append("AccessToken", "", accessOptions);          
            Response.Cookies.Append("RefreshToken", "", refreshOptions);

            return Ok(new { message = "Logout effettuato. Sessione ed hardware-token invalidati." });
        }
    }
}
