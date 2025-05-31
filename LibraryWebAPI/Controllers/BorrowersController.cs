// LibraryWebAPI/Controllers/BorrowersController.cs
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
    public class BorrowersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BorrowersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Borrowers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowerDto>>> GetBorrowers()
        {
            return await _context.Borrowers
                .Select(b => new BorrowerDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Email = b.Email,
                    Phone = b.Phone
                })
                .ToListAsync();
        }

        // GET: api/Borrowers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowerDto>> GetBorrower(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);

            if (borrower == null)
            {
                return NotFound();
            }

            return new BorrowerDto
            {
                Id = borrower.Id,
                Name = borrower.Name,
                Email = borrower.Email,
                Phone = borrower.Phone
            };
        }

        // POST: api/Borrowers
        [HttpPost]
        public async Task<ActionResult<BorrowerDto>> PostBorrower([FromBody] BorrowerCreateDto borrowerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for existing email
            if (await _context.Borrowers.AnyAsync(b => b.Email == borrowerDto.Email))
            {
                return Conflict(new { message = "A borrower with this email already exists." });
            }

            var borrower = new Borrower
            {
                Name = borrowerDto.Name,
                Email = borrowerDto.Email,
                Phone = borrowerDto.Phone
            };

            _context.Borrowers.Add(borrower);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBorrower", new { id = borrower.Id }, new BorrowerDto
            {
                Id = borrower.Id,
                Name = borrower.Name,
                Email = borrower.Email,
                Phone = borrower.Phone
            });
        }

        // PUT: api/Borrowers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBorrower(int id, [FromBody] BorrowerUpdateDto borrowerDto)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(borrowerDto.Name)) borrower.Name = borrowerDto.Name;
            if (!string.IsNullOrEmpty(borrowerDto.Email))
            {
                // Check if the new email conflicts with another existing borrower (excluding itself)
                if (await _context.Borrowers.AnyAsync(b => b.Email == borrowerDto.Email && b.Id != id))
                {
                    return Conflict(new { message = "Another borrower with this email already exists." });
                }
                borrower.Email = borrowerDto.Email;
            }
            if (!string.IsNullOrEmpty(borrowerDto.Phone)) borrower.Phone = borrowerDto.Phone;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowerExists(id))
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

        // DELETE: api/Borrowers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrower(int id)
        {
            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }

            // Check if there are any active loans for this borrower
            var activeLoans = await _context.Loans.AnyAsync(l => l.BorrowerId == id && l.ReturnDate == null);
            if (activeLoans)
            {
                return BadRequest(new { message = "Cannot delete borrower while there are active loans associated with them." });
            }

            _context.Borrowers.Remove(borrower);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BorrowerExists(int id)
        {
            return _context.Borrowers.Any(e => e.Id == id);
        }
    }
}