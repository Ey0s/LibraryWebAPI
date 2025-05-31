// LibraryWebAPI/Models/Book.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [StringLength(17, MinimumLength = 10)] // ISBN-10 or ISBN-13 format
        public string ISBN { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; } // Derived or managed by business logic

        // Navigation property for related loans (optional, but good for EF Core)
        public ICollection<Loan>? Loans { get; set; }
    }
}