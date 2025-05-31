// LibraryWebAPI/Models/Borrower.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class Borrower
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        // Navigation property for related loans
        public ICollection<Loan>? Loans { get; set; }
    }
}