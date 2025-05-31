// LibraryWebAPI/Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // Store hashed password
    }
}