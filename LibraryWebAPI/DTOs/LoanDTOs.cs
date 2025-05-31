// LibraryWebAPI/DTOs/LoanDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.DTOs
{
    public class IssueLoanRequest
    {
        [Required(ErrorMessage = "Book ID is required.")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Borrower ID is required.")]
        public int BorrowerId { get; set; }
    }

    public class ReturnLoanRequest
    {
        [Required(ErrorMessage = "Loan ID is required.")]
        public int LoanId { get; set; }
    }

    public class LoanDto // Used for returning loan details
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty; // Populated from Book
        public int BorrowerId { get; set; }
        public string BorrowerName { get; set; } = string.Empty; // Populated from Borrower
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsOverdue { get; set; } // Derived property, not stored in DB
    }
}