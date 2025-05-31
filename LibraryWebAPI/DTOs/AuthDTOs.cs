// LibraryWebAPI/DTOs/AuthDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}