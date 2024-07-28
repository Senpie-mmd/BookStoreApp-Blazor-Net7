using BookStoreApp.API.Data;
using BookStoreApp.API.Utilitis;
using BookStoreApp.API.ViewModels.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        #region Fields

        private readonly BookStoreAppDbContext _context;
        private readonly ILogger<AuthorsController> _logger;

        #endregion

        #region Ctor

        public AuthorsController(BookStoreAppDbContext context,
            ILogger<AuthorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorViewModel>>> GetAuthors()
        {
            try
            {
                if (_context.Authors == null)
                {
                    return NotFound();
                }

                var authors = await _context.Authors.ToListAsync();

                var authorsDto = authors.Select(n => new AuthorViewModel()
                {
                    Id = n.Id,
                    FirstName = n.FirstName,
                    LastName = n.LastName,
                    Bio = n.Bio
                }).ToList();
                return Ok(authorsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(GetAuthors)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorViewModel>> GetAuthor(int id)
        {
            try
            {
                if (_context.Authors == null)
                {
                    return NotFound();
                }
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    return NotFound();
                }

                var authorDto = new AuthorViewModel()
                {
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    Bio = author.Bio
                };

                return Ok(authorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(GetAuthor)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, UpdateAuthorViewModel authorDto)
        {
            if (id != authorDto.Id)
            {
                return BadRequest();
            }
            var author = new Author
            {
                Id = authorDto.Id,
                FirstName = authorDto.FirstName,
                LastName = authorDto.LastName,
                Bio = authorDto.Bio
            };

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(PutAuthor)}");
                return StatusCode(500, Message.Error500Message);
            }

            return NoContent();
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AuthorViewModel>> PostAuthor(CreateNewAuthorViewModel authorDto)
        {
            try
            {
                if (_context.Authors == null)
                {
                    return Problem("Entity set 'BookStoreAppDbContext.Authors'  is null.");
                }
                var author = new Author()
                {
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Bio = authorDto.Bio
                };

                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(PostAuthor)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                if (_context.Authors == null)
                {
                    return NotFound();
                }
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(DeleteAuthor)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(e => e.Id == id);
        }
    }
}
