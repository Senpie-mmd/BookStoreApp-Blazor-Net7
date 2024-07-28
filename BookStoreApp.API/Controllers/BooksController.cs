using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.API.Data;
using BookStoreApp.API.ViewModels.Book;
using BookStoreApp.API.Utilitis;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        #region Fields

        private readonly BookStoreAppDbContext _context;
        private readonly ILogger<BooksController> _logger;

        #endregion

        #region Ctor

        public BooksController(BookStoreAppDbContext context,
            ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #endregion

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookViewModel>>> GetBooks()
        {
            try
            {
                if (_context.Books == null)
                {
                    return NotFound();
                }

                var books = await _context.Books.Include(n => n.Author).ToListAsync();
                var booksDTo = books.Select(n => new BookViewModel()
                {
                    Id = n.Id,
                    AuthorId = (int)n.AuthorId,
                    AuthorName = n.Author.FirstName + " " + n.Author.LastName,
                    Image = n.Image,
                    Price = (decimal)n.Price,
                    Title = n.Title,
                    Year = (int)n.Year
                }).ToList();
                return Ok(booksDTo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(GetBooks)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailsViewModel>> GetBook(int id)
        {
            try
            {
                if (_context.Books == null)
                {
                    return NotFound();
                }
                var book = await _context.Books.Include(n => n.Author).FirstOrDefaultAsync(n => n.Id == id);

                if (book == null)
                {
                    return NotFound();
                }

                var bookDto = new BookDetailsViewModel()
                {
                    Id = book.Id,
                    AuthorId = (int)book.AuthorId,
                    AuthorName = book.Author.FirstName + " " + book.Author.LastName,
                    Image = book.Image,
                    Isbn = book.Isbn,
                    Price = (decimal)book.Price,
                    Summary = book.Summary,
                    Title = book.Title,
                    Year = (int)book.Year
                };

                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(GetBook)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, UpdateBookViewModel bookDto)
        {
            if (id != bookDto.Id)
            {
                return BadRequest();
            }

            var book = new Book()
            {
                Id = bookDto.Id,
                AuthorId = bookDto.AuthorId,
                Image = bookDto.Image,
                Isbn = bookDto.Isbn,
                Price = bookDto.Price,
                Summary = bookDto.Summary,
                Title = bookDto.Title,
                Year = bookDto.Year
            };

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(PutBook)}");
                return StatusCode(500, Message.Error500Message);
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookDetailsViewModel>> PostBook(CreateNewBookViewModel bookDto)
        {
            try
            {
                if (_context.Books == null)
                {
                    return Problem("Entity set 'BookStoreAppDbContext.Books'  is null.");
                }

                var book = new Book()
                {
                    AuthorId = bookDto.AuthorId,
                    Summary = bookDto.Summary,
                    Image = bookDto.Image,
                    Isbn = bookDto.Isbn,
                    Price = bookDto.Price,
                    Title = bookDto.Title,
                    Year = bookDto.Year,
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(PostBook)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                if (_context.Books == null)
                {
                    return NotFound();
                }
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(DeleteBook)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        private async Task<bool> BookExists(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }
    }
}
