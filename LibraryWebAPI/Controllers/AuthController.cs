// LibraryWebAPI/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using LibraryWebAPI.DTOs; // Import your DTOs namespace
using LibraryWebAPI.Services; // Import your Services namespace
using Microsoft.AspNetCore.Authorization; // For [AllowAnonymous] and [Authorize]

namespace LibraryWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous] // Allow unauthenticated access
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.Register(request.Username, request.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username already exists." });
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new AuthResponse { Username = user.Username, Token = token });
        }

        [AllowAnonymous] // Allow unauthenticated access
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.Login(request.Username, request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new AuthResponse { Username = user.Username, Token = token });
        }
    }
}