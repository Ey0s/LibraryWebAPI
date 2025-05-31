// LibraryWebAPI/DTOs/BookDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.DTOs
{
    public class BookCreateDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2 and 100 characters.")]
        public string Author { get; set; } = string.Empty;

        [Required]
        [StringLength(17, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 17 characters (including hyphens).")] // ISBN-10 or ISBN-13
        public string ISBN { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1.")]
        public int TotalCopies { get; set; }
    }

    public class BookUpdateDto
    {
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
        public string? Title { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2 and 100 characters.")]
        public string? Author { get; set; }

        [StringLength(17, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 17 characters (including hyphens).")]
        public string? ISBN { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Total copies cannot be negative.")]
        public int? TotalCopies { get; set; }
    }

    public class BookDto // Used for returning book details
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
    }
}