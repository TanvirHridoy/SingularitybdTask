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
    public class TrashController : ControllerBase
    {
        private readonly DownDbTestContext _context;

        public TrashController(DownDbTestContext context)
        {
            _context = context;
        }

        // GET: api/Trash
        [HttpGet]
        public async Task<ActionResult<IEnumerable<File>>> GetFiles()
        {
            AspNetUser user = await _context.AspNetUsers.SingleAsync(e => e.UserName == User.Identity.Name);

            List<File> files = new List<File>();

            var list = await _context.Files.Where(e => e.IsDeleted == true && e.OwnerUserId == user.Id).ToListAsync();
            foreach (var item in list)
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

        // GET: api/Trash/5
        [HttpGet("{id}")]
        public async Task<ActionResult<File>> GetFile(int id)
        {

            AspNetUser user = await _context.AspNetUsers.SingleAsync(e => e.UserName == User.Identity.Name);
            var file = await _context.Files.SingleAsync(e => e.IsDeleted == true && e.Id == id && e.OwnerUserId == user.Id);
            file.OwnerUser = null;
            if (file == null)
            {
                return NotFound();
            }

            return file;
        }

        // PUT: api/Trash/5
        //This method is used to restore file
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFile(int id, File file)
        {
            AspNetUser user = await _context.AspNetUsers.SingleAsync(e => e.UserName == User.Identity.Name);
            File dfile = await _context.Files.SingleAsync(e => e.Id == id && e.OwnerUserId == user.Id && e.IsDeleted == true);
            //dfile.OwnerUser = null;
            if (dfile==null)
            {
                return BadRequest();
            }
            dfile.IsDeleted = false;
            dfile.DeleteDate = null;
            _context.Entry(dfile).State = EntityState.Modified;

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

        

        // DELETE: api/Trash/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            AspNetUser user = await _context.AspNetUsers.SingleAsync(e => e.UserName == User.Identity.Name);
            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }
            if (file.OwnerUserId != user.Id)
            {
                return BadRequest("This file doesn't belong to you");
            }

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FileExists(int id)
        {
            return _context.Files.Any(e => e.Id == id);
        }
    }
}
