using LucaHome.DTO;
using LucaHome.Services;
using LucaHome.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LucaHome.Controllers
{
    [Route("api/loginpqc")]
    [ApiController]
    public class PqcController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPqcService _pqcService;

        public PqcController(IUserService userService, IPqcService pqcService)
        { 
            _userService = userService; 
            _pqcService = pqcService;
        }

        // 1. GET api/login/handshake -> Astro recupera la chiave pubblica del server
        [HttpGet("handshake")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Handshake()
        {
            // Otteniamo la chiave pubblica dal tuo servizio
            var publicKeyBytes = _pqcService.GetPublicKeyBytes();

            // Convertiamo in Base64 per spedirla via JSON ad Astro
            string publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);

            return Ok(new
            {
                // In questa versione non usiamo un sessionId dinamico sul server:
                // la chiave privata è fissa in memoria sul PqcService (registrato come Singleton)
                PublicKeyBase64 = publicKeyBase64
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [EnableRateLimiting("strict")]
        public async Task<IActionResult> Login(UserDTOIn user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Ciphertext))
                return Unauthorized();

            // Aggiunto handshake
            try
            {
                // Convertiamo il ciphertext in arrivo dal front da Base64 a byte[]
                byte[] ciphertextBytes = Convert.FromBase64String(user.Ciphertext);

                // Chiamiamo il tuo metodo originale!
                _pqcService.FinalizeHandshake(ciphertextBytes);
            }
            catch (Exception)
            {
                // Se la decapsulazione fallisce (es. chiavi non corrispondenti o corrotte), blocchiamo subito
                return Unauthorized();
            }

            var token = await _userService.Login(user);

            if (token == null) return Unauthorized();

            return Ok(new { Token = token });
        }        
    }
}
