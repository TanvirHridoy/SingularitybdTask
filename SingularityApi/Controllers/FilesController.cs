using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SingularityApi.Models;

namespace SingularityApi.Controllers
{
    [Authorize(AuthenticationSchemes = ApiConst.AuthSchemes, Roles = "Admin,Manager,Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly DownDbTestContext _context;

        public FilesController(DownDbTestContext context)
        {
            _context = context;
        }

        // GET: api/Files
        [HttpGet]
        public async Task<ActionResult<IEnumerable<File>>> GetFiles()
        {
            AspNetUser user = await _context.AspNetUsers.SingleAsync(e => e.UserName == User.Identity.Name);

            List<File> files = new List<File>();
                
             var list=   await _context.Files.Where(e=>e.IsDeleted==false && e.OwnerUserId==user.Id).ToListAsync();
            foreach(var item in list)
            {
                File temp = new File
                {
                    DeleteDate = item.DeleteDate,
                    OwnerUser = null,
                    FileName = item.FileName,
                    FileUrl = item.FileUrl,
                    Id = item.Id,
                    IsDeleted = item.IsDeleted,
                    OwnerUserId = item.OwnerUserId
                };
                files.Add(temp);
            }
            return files;
        }

        // GET: api/Files/5
        [HttpGet("{id}")]
        public async Task<ActionResult<File>> GetFile(int id)
        {
            AspNetUser user = await _context.AspNetUsers.SingleAsync(e => e.UserName == User.Identity.Name);
            var file = await _context.Files.SingleAsync(e=>e.IsDeleted==false && e.Id==id && e.OwnerUserId==user.Id);
            file.OwnerUser = null;
            if (file == null)
            {
                return NotFound();
            }

            return file;
        }

        // PUT: api/Files/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFile(int id, File file)
        {
            AspNetUser user = await _context.AspNetUsers.SingleAsync(e => e.UserName == User.Identity.Name);
            if (user.Id != file.OwnerUserId)
            {
                return BadRequest("You dont have access to the file");
            }
            if (id != file.Id)
            {
                return BadRequest();
            }

            _context.Entry(file).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileExists(id))
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

        // POST: api/Files
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<File>> PostFile(File file)
        {
            AspNetUser user = await _context.AspNetUsers.SingleAsync(e => e.UserName == User.Identity.Name);
            file.OwnerUserId = user.Id;
            _context.Files.Add(file);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FileExists(file.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(200);
        }

        // DELETE: api/Files/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }
            file.IsDeleted = true;
            file.DeleteDate = DateTime.Now;
            _context.Entry(file).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FileExists(int id)
        {
            return _context.Files.Any(e => e.Id == id);
        }
    }
}
