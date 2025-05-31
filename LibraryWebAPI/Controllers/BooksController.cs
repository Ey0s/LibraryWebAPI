// LibraryWebAPI/Controllers/BooksController.cs
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
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            return await _context.Books
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies // This is managed by business logic
                })
                .ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies
            };
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookDto>> PostBook([FromBody] BookCreateDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for existing ISBN
            if (await _context.Books.AnyAsync(b => b.ISBN == bookDto.ISBN))
            {
                return Conflict(new { message = "A book with this ISBN already exists." });
            }

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                TotalCopies = bookDto.TotalCopies,
                AvailableCopies = bookDto.TotalCopies // Initially, all copies are available
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies
            });
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromBody] BookUpdateDto bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Calculate current borrowed copies
            var borrowedCopies = book.TotalCopies - book.AvailableCopies;

            // Only update fields if they are provided in the DTO
            if (!string.IsNullOrEmpty(bookDto.Title)) book.Title = bookDto.Title;
            if (!string.IsNullOrEmpty(bookDto.Author)) book.Author = bookDto.Author;
            if (!string.IsNullOrEmpty(bookDto.ISBN))
            {
                // Check if the new ISBN conflicts with another existing book (excluding itself)
                if (await _context.Books.AnyAsync(b => b.ISBN == bookDto.ISBN && b.Id != id))
                {
                    return Conflict(new { message = "Another book with this ISBN already exists." });
                }
                book.ISBN = bookDto.ISBN;
            }

            if (bookDto.TotalCopies.HasValue)
            {
                if (bookDto.TotalCopies.Value < borrowedCopies)
                {
                    return BadRequest(new { message = $"Cannot reduce total copies below currently borrowed copies ({borrowedCopies})." });
                }
                book.TotalCopies = bookDto.TotalCopies.Value;
                book.AvailableCopies = book.TotalCopies - borrowedCopies;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Check if there are any active loans for this book
            var activeLoans = await _context.Loans.AnyAsync(l => l.BookId == id && l.ReturnDate == null);
            if (activeLoans)
            {
                return BadRequest(new { message = "Cannot delete book while there are active loans associated with it." });
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}