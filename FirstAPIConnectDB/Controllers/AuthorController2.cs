using FirstAPIConnectDB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstAPIConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController2 : ControllerBase
    {

        private readonly LibraryDbContext _context;
        private readonly ILogger<AuthorController2> _logger;

        public AuthorController2(LibraryDbContext context, ILogger<AuthorController2> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all authors
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            try
            {
                var authors = await _context.Authors
                    .Include(a => a.Books)
                    .ToListAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving authors");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get a specific author by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            try
            {
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.AuthorId == id);

                if (author == null)
                {
                    return NotFound($"Author with ID {id} not found");
                }

                return Ok(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving author with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new author
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(Author author)
        {
            try
            {
                if (author == null)
                {
                    return BadRequest("Author cannot be null");
                }

                if (string.IsNullOrWhiteSpace(author.Name))
                {
                    return BadRequest("Author name is required");
                }

                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAuthor", new { id = author.AuthorId }, author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating author");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing author
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            try
            {
                if (id != author.AuthorId)
                {
                    return BadRequest("ID mismatch");
                }

                if (string.IsNullOrWhiteSpace(author.Name))
                {
                    return BadRequest("Author name is required");
                }

                _context.Entry(author).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(id))
                    {
                        return NotFound($"Author with ID {id} not found");
                    }
                    throw;
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating author with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete an author
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound($"Author with ID {id} not found");
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting author with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(a => a.AuthorId == id);
        }
    }
}
