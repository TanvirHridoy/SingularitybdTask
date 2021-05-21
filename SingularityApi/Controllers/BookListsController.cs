using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SingularityApi.Models;

namespace SingularityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = ApiConst.AuthSchemes, Roles = "Admin,Manager,Employee")]
    public class BookListsController : ControllerBase
    {
        private readonly DownDbTestContext _context;

        public BookListsController(DownDbTestContext context)
        {
            _context = context;
        }

        // GET: api/BookLists

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookList>>> GetBookLists()
        {
            var x = User.Identity.Name;
            return await _context.BookLists.ToListAsync();
        }


        // GET: api/BookLists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookList>> GetBookList(int id)
        {


            var bookList = await _context.BookLists.FindAsync(id);

            if (bookList == null)
            {
                return NotFound();
            }

            return bookList;
        }

        // PUT: api/BookLists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookList(int id, BookList bookList)
        {
            if (id != bookList.Id)
            {
                return BadRequest();
            }

            _context.Entry(bookList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookListExists(id))
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

        // POST: api/BookLists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [Authorize]
        [HttpPost]
        public async Task<ActionResult<BookList>> PostBookList(BookList bookList)
        {

            _context.BookLists.Add(bookList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookList", new { id = bookList.Id }, bookList);
        }

        // DELETE: api/BookLists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookList(int id)
        {
            var bookList = await _context.BookLists.FindAsync(id);
            if (bookList == null)
            {
                return NotFound();
            }

            _context.BookLists.Remove(bookList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookListExists(int id)
        {
            return _context.BookLists.Any(e => e.Id == id);
        }
    }
}
