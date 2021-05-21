using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SingularitybdWeb.ApiModels;
using SingularitybdWeb.Data;
using SingularitybdWeb.Models;
using SingularitybdWeb.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SingularitybdWeb.Controllers
{
    //[Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDataAccessService _dataAccessService;
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _DbContext;
        public AdminController(
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IDataAccessService dataAccessService,
                ILogger<AdminController> logger,
                ApplicationDbContext applicationDbContext,
                SignInManager<IdentityUser> signInManager
                )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataAccessService = dataAccessService;
            _logger = logger;
            _DbContext = applicationDbContext;
            _signInManager = signInManager;
        }

        [Authorize("Authorization")]
        public async Task<IActionResult> Roles()
        {
            var roleViewModel = new List<RoleViewModel>();

            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                foreach (var item in roles)
                {
                    roleViewModel.Add(new RoleViewModel()
                    {
                        Id = item.Id,
                        Name = item.Name,
                    });
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.GetBaseException().Message);
            }

            return View(roleViewModel);
        }

        [Authorize("Roles")]
        public IActionResult CreateRole()
        {
            return View(new RoleViewModel());
        }

        [HttpPost]
        [Authorize("Roles")]
        public async Task<IActionResult> CreateRole(RoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole() { Name = viewModel.Name });
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Roles));
                }
                else
                {
                    ModelState.AddModelError("Name", string.Join(",", result.Errors));
                }
            }

            return View(viewModel);
        }

        [Authorize("Authorization")]
        public async Task<IActionResult> Users()
        {
            var userViewModel = new List<UserViewModel>();

            try
            {
                var users = await _userManager.Users.ToListAsync();
                foreach (var item in users)
                {
                    userViewModel.Add(new UserViewModel()
                    {
                        Id = item.Id,
                        Email = item.Email,
                        UserName = item.UserName,
                    });
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.GetBaseException().Message);
            }

            return View(userViewModel);
        }

        [Authorize("Users")]
        public async Task<IActionResult> EditUser(string id)
        {
            var viewModel = new UserViewModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                var userRoles = await _userManager.GetRolesAsync(user);

                viewModel.Email = user?.Email;
                viewModel.UserName = user?.UserName;

                var allRoles = await _roleManager.Roles.ToListAsync();
                viewModel.Roles = allRoles.Select(x => new RoleViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Selected = userRoles.Contains(x.Name)
                }).ToArray();

            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize("Users")]
        public async Task<IActionResult> EditUser(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(viewModel.Id);
                var userRoles = await _userManager.GetRolesAsync(user);

                await _userManager.RemoveFromRolesAsync(user, userRoles);
                await _userManager.AddToRolesAsync(user, viewModel.Roles.Where(x => x.Selected).Select(x => x.Name));

                return RedirectToAction(nameof(Users));
            }

            return View(viewModel);
        }

        [Authorize("Authorization")]
        public async Task<IActionResult> EditRolePermission(string id)
        {
            var permissions = new List<NavigationMenuViewModel>();
            if (!string.IsNullOrWhiteSpace(id))
            {
                permissions = await _dataAccessService.GetPermissionsByRoleIdAsync(id);
            }

            return View(permissions);
        }

        [HttpPost]
        [Authorize("Authorization")]
        public async Task<IActionResult> EditRolePermission(string id, List<NavigationMenuViewModel> viewModel)
        {
            if (ModelState.IsValid)
            {
                var permissionIds = viewModel.Where(x => x.Permitted).Select(x => x.Id);

                await _dataAccessService.SetPermissionsByRoleIdAsync(id, permissionIds);
                return RedirectToAction(nameof(Roles));
            }

            return View(viewModel);
        }


        [Authorize("Authorization")]
        public async Task<IActionResult> ViewMenu(string status)
        {
            MenuViewModel viewModel = new MenuViewModel();
            viewModel.Status = status;
            viewModel.menus = await _DbContext.NavigationMenu.ToListAsync();
            return View(viewModel);
        }


        [HttpGet]
        [Authorize("Authorization")]
        public async Task<IActionResult> AddMenu()
        {
            AddMenuModel viewModel = new AddMenuModel();
            viewModel.ParentMenus= await _DbContext.NavigationMenu.ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize("Authorization")]
        public async Task<IActionResult> AddMenu(AddMenuModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _DbContext.NavigationMenu.Where(e => e.Name == model.Name & e.ActionName == model.ActionName && e.ControllerName == e.ControllerName).ToListAsync();

                if (result.Count() != 0)
                {
                    return RedirectToAction("ViewMenu", new { status = "Menu Already exists" });
                }
                else
                {
                    NavigationMenu menu = new NavigationMenu
                    {
                        Id = new Guid(),
                        Name = model.Name,
                        ActionName = model.ActionName,
                        Area = model.Area,
                        ControllerName = model.ControllerName,
                        DisplayOrder = model.DisplayOrder,
                        ExternalUrl = model.ExternalUrl,
                        IsExternal = model.IsExternal,
                        ParentMenuId = model.ParentMenuId,
                        Permitted = model.Permitted,
                        Visible = model.Visible

                    };


                    await _DbContext.NavigationMenu.AddAsync(menu);

                    _DbContext.SaveChangesAsync().Wait();
                }
               
            }
            

            return RedirectToAction("ViewMenu", new { status = "Added Succesfully" });

        }
        

        [Authorize("Authorization")]
        public async Task<IActionResult> EditMenu(string id)
        {
           
                var result = await _DbContext.NavigationMenu.SingleAsync(e => e.Id.ToString() == id.Trim());

                if (result == null)
                {
                    return RedirectToAction("ViewMenu", new { status = "Edit Failed" });
                }
                else
                
                return View(result);
        }

        [Authorize("Authorization")]
        public async Task<IActionResult> UpdateMenu(NavigationMenu model)
        {
            if (ModelState.IsValid)
            {
                 _DbContext.NavigationMenu.Update(model);
                await _DbContext.SaveChangesAsync();
                return RedirectToAction("ViewMenu", new { status = "Update successfull" });
            }
            else
                return RedirectToAction("ViewMenu", new { status = "Failed" });
        }


        [Authorize("Authorization")]
        public async Task<IActionResult> DeleteMenu(string Id)
        {
            var menu = await _DbContext.NavigationMenu.SingleAsync(e => e.Id.ToString() == Id.Trim());

            if (menu != null)
            {
                 _DbContext.NavigationMenu.Remove(menu);

                _DbContext.SaveChangesAsync().Wait();
                return RedirectToAction("ViewMenu", new { status = "Deleted Succesfully" });
            }
            return RedirectToAction("ViewMenu", new { status = "Failed To Deleted " });
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] JwtTokenViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByNameAsync(model.UserName);
                var Signresult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                
                var userRoles = await _userManager.GetRolesAsync(user);
                
                if (Signresult.Succeeded)
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiJwtConst.key));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim> ();

                   claims.Add( new Claim(JwtRegisteredClaimNames.Sub, model.UserName));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, model.UserName));
                    
                    foreach (var role in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var token = new JwtSecurityToken(
                           ApiJwtConst.Issuer,
                           ApiJwtConst.Audience,
                           claims,
                           expires: DateTime.UtcNow.AddMinutes(30),
                           signingCredentials: creds

                        );
                    var result = new
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
    }
}