using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SingularityApi.Models;

namespace SingularityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DownDbTestContext _context;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;
        public AccountController(DownDbTestContext context, UserManager<AspNetUser> userManager, SignInManager<AspNetUser> sngMngr)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = sngMngr;
        }

        [HttpPost("{username}")]
        public async Task<IActionResult> RefreshToken(string username, string val = "")
        {
            if (ModelState.IsValid)
            {
                AspNetUser user = new AspNetUser();

                user = await _userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return BadRequest("No user found");

                }
                List<string> userRoles = new List<string>();
                //var userRoles = await _userManager.GetRolesAsync(user);
                var LstuserRoles = await _context.AspNetUserRoles.Where(e => e.UserId == user.Id).ToListAsync();
                foreach (var item in LstuserRoles)
                {
                    var t = await _context.AspNetRoles.Where(e => e.Id == item.RoleId).SingleAsync();
                    if (t != null)
                    {
                        userRoles.Add(t.Name);
                    }
                }
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiConst.key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>();

                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.UserName));
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName));

                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = new JwtSecurityToken(
                       ApiConst.Issuer,
                       ApiConst.Audience,
                       claims,
                       expires: DateTime.UtcNow.AddMinutes(30),
                       signingCredentials: creds

                    );
                var result = new Token
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                };
                return Created("", result);

            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] JwtTokenViewModel model)
        {
            if (ModelState.IsValid)
            {
                AspNetUser user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return BadRequest("no user found");
                }

                // var IsSuccess= Hasher.VerifyHashedPassword(user.PasswordHash, model.Password);
                var Signresult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                List<string> userRoles = new List<string>();
                //var userRoles = await _userManager.GetRolesAsync(user);
                var LstuserRoles = await _context.AspNetUserRoles.Where(e => e.UserId == user.Id).ToListAsync();
                foreach (var item in LstuserRoles)
                {
                    var t = await _context.AspNetRoles.Where(e => e.Id == item.RoleId).SingleAsync();
                    if (t != null)
                    {
                        userRoles.Add(t.Name);
                    }
                }


                if (Signresult.Succeeded)
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiConst.key));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>();

                    claims.Add(new Claim(JwtRegisteredClaimNames.Sub, model.UserName));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, model.UserName));

                    foreach (var role in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var token = new JwtSecurityToken(
                           ApiConst.Issuer,
                           ApiConst.Audience,
                           claims,
                           expires: DateTime.UtcNow.AddMinutes(30),
                           signingCredentials: creds

                        );
                    var result = new Token
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    };
                    return Created("", result);
                }
                else
                {
                    return BadRequest("Invalid credential");
                }
            }

            return BadRequest();

        }


        private bool AspNetUserExists(string id)
        {
            return _context.AspNetUsers.Any(e => e.Id == id);
        }
    }
}
