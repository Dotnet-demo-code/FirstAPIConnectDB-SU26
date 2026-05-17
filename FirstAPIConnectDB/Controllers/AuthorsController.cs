using FirstAPIConnectDB.DTOs;
using FirstAPIConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace FirstAPIConnectDB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly ILogger<AuthorsController> _logger;
        private readonly IMapper _mapper;

        public AuthorsController(
            LibraryDbContext context
            , ILogger<AuthorsController> logger
            , IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all authors
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            try
            {
                var authors = await _context.Authors
                    //.Skip(3) // thao tác tren danh sách
                    .Include(a => a.Books) // thao tac DB
                    .ToListAsync();

                List<AuthorDTO> authorDTOs = _mapper.Map<List<AuthorDTO>>(authors);
                return Ok(authorDTOs);
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
        public async Task<ActionResult<Author>> PostAuthor(AuthorAddDTO author)
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
                                
                var authorEntity = _mapper.Map<Author>(author);

                _context.Authors.Add(authorEntity);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAuthor", new { id = authorEntity.AuthorId }, author);
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

                // _context.Entry(author).State = EntityState.Modified;
                _context.Authors.Update(author);
                try
                {
                    await _context.SaveChangesAsync(); // luu sau khi update
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
