// LibraryWebAPI/Models/Loan.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!; // Navigation property

        [Required]
        public int BorrowerId { get; set; }
        public Borrower Borrower { get; set; } = null!; // Navigation property

        public DateTime LoanDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; } // Set upon loan creation, e.g., 14 days from LoanDate
        public DateTime? ReturnDate { get; set; } // Null if not returned yet

        public bool IsOverdue => ReturnDate == null && DateTime.UtcNow > DueDate;
    }
}