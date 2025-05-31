// LibraryWebAPI/Controllers/LoansController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryWebAPI.Data;
using LibraryWebAPI.Models;
using LibraryWebAPI.DTOs;
using Microsoft.AspNetCore.Authorization; // For [Authorize]

namespace LibraryWebAPI.Controllers
{
    [Authorize] // All actions in this controller require authentication
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Loans - Get all loans (active and returned)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Borrower)
                .Select(l => new LoanDto
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book.Title,
                    BorrowerId = l.BorrowerId,
                    BorrowerName = l.Borrower.Name,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    IsOverdue = l.IsOverdue // This property is calculated in the model
                })
                .OrderByDescending(l => l.LoanDate) // Order by latest loans first
                .ToListAsync();
        }

        // GET: api/Loans/Overdue - Get only overdue loans
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdueLoans()
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Borrower)
                .Where(l => l.ReturnDate == null && l.DueDate < DateTime.UtcNow) // Filter for active and overdue
                .Select(l => new LoanDto
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book.Title,
                    BorrowerId = l.BorrowerId,
                    BorrowerName = l.Borrower.Name,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    IsOverdue = l.IsOverdue
                })
                .OrderBy(l => l.DueDate) // Order by most overdue first
                .ToListAsync();
        }


        // POST: api/Loans - Issue a new loan
        [HttpPost]
        public async Task<ActionResult<LoanDto>> IssueLoan([FromBody] IssueLoanRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            var borrower = await _context.Borrowers.FindAsync(request.BorrowerId);
            if (borrower == null)
            {
                return NotFound(new { message = "Borrower not found." });
            }

            if (book.AvailableCopies <= 0)
            {
                return BadRequest(new { message = "No available copies of this book." });
            }

            var loan = new Loan
            {
                BookId = request.BookId,
                BorrowerId = request.BorrowerId,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14) // Default loan period: 14 days
            };

            book.AvailableCopies--; // Decrease available copies

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            // Fetch the loan again with navigation properties to get full DTO data
            await _context.Entry(loan).Reference(l => l.Book).LoadAsync();
            await _context.Entry(loan).Reference(l => l.Borrower).LoadAsync();


            return CreatedAtAction("GetLoans", new { id = loan.Id }, new LoanDto
            {
                Id = loan.Id,
                BookId = loan.BookId,
                BookTitle = loan.Book.Title,
                BorrowerId = loan.BorrowerId,
                BorrowerName = loan.Borrower.Name,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                IsOverdue = loan.IsOverdue
            });
        }

        // POST: api/Returns - Return a book
        [HttpPost("~/api/Returns")] // Separate endpoint for returns, or can be PUT to /api/Loans/{id}/return
        public async Task<IActionResult> ReturnBook([FromBody] ReturnLoanRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loan = await _context.Loans
                               .Include(l => l.Book) // Include book to update its available copies
                               .SingleOrDefaultAsync(l => l.Id == request.LoanId && l.ReturnDate == null); // Ensure it's an active loan

            if (loan == null)
            {
                return NotFound(new { message = "Active loan not found or already returned." });
            }

            loan.ReturnDate = DateTime.UtcNow;
            loan.Book.AvailableCopies++; // Increase available copies

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}