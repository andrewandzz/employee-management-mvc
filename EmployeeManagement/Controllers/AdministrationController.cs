using EmployeeManagement.Models;
using EmployeeManagement.ViewModels.Administration;
using EmployeeManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Route("administration")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        [Route("list-users")]
        public IActionResult ListUsers()
        {
            return View(userManager.Users);
        }

        [HttpGet]
        [Authorize(Policy = "EditPolicy")]
        [Route("edit-user/{id}")]
        public async Task<IActionResult> EditUser(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return UserNotFoundView(id);
            }

            IList<Claim> userClaims = await userManager.GetClaimsAsync(user);
            IList<string> userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                City = user.City,
                Claims = (from claim in userClaims select claim.Value).ToList(),
                RoleNames = userRoles
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditPolicy")]
        [Route("edit-user/{id}")]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return UserNotFoundView(model.Id);
            }

            user.UserName = model.Name;
            user.Email = model.Email;
            user.City = model.City;

            IdentityResult result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "DeletePolicy")]
        [Route("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return UserNotFoundView(id);
            }

            IdentityResult result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("ListUsers");
        }

        [HttpGet]
        [Authorize(Policy = "EditAdminPolicy")]
        [Route("edit-user-roles")]
        public async Task<IActionResult> EditUserRoles(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return UserNotFoundView(id);
            }

            ViewBag.UserId = user.Id;

            var model = new List<UserRoleViewModel>();

            List<IdentityRole> allRoles = await roleManager.Roles.ToListAsync();

            foreach (IdentityRole role in allRoles)
            {
                var userRole = new UserRoleViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsRoleSelected = await userManager.IsInRoleAsync(user, role.Name)
                };

                model.Add(userRole);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditAdminPolicy")]
        [Route("edit-user-roles/{userId}")]
        public async Task<IActionResult> EditUserRoles(List<UserRoleViewModel> model, string userId)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return UserNotFoundView(userId);
            }

            IList<string> roleNames = await userManager.GetRolesAsync(user);
            IdentityResult removeResult = await userManager.RemoveFromRolesAsync(user, roleNames);

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Can not remove existing user roles.");
                return View(model);
            }

            IEnumerable<string> rolesNamesToAdd = model.Where(m => m.IsRoleSelected).Select(m => m.RoleName);
            IdentityResult addResult = await userManager.AddToRolesAsync(user, rolesNamesToAdd);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Can not add selected roles to the user.");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });
        }

        [HttpGet]
        [Authorize(Policy = "EditAdminPolicy")]
        [Route("edit-user-claims/{id}")]
        public async Task<IActionResult> EditUserClaims(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return UserNotFoundView(id);
            }

            var model = new EditUserClaimsViewModel()
            {
                UserId = user.Id
            };

            IList<Claim> userCurrentClaims = await userManager.GetClaimsAsync(user);

            foreach (Claim claim in Claims.All)
            {
                var userClaim = new UserClaim()
                {
                    Type = claim.Type,
                    Value = claim.Value,
                    IsSelected = userCurrentClaims.Any(c => c.Value == claim.Value)
                };

                model.Claims.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditAdminPolicy")]
        [Route("edit-user-claims/{id}")]
        public async Task<IActionResult> EditUserClaims(EditUserClaimsViewModel model)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return UserNotFoundView(model.UserId);
            }

            IList<Claim> userCurrentClaims = await userManager.GetClaimsAsync(user);
            IdentityResult removeResult = await userManager.RemoveClaimsAsync(user, userCurrentClaims);

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Can not remove existing user claims.");
                return View(model);
            }

            IEnumerable<Claim> claimsToAdd = model.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.Type, c.Value));
            IdentityResult addResult = await userManager.AddClaimsAsync(user, claimsToAdd);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Can not add selected claims to the user.");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = model.UserId});
        }

        [HttpGet]
        [Authorize(Policy = "CreatePolicy")]
        [Route("create-role")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CreatePolicy")]
        [Route("create-role")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole(model.RoleName);
                IdentityResult result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "ReadPolicy")]
        [Route("list-roles")]
        public IActionResult ListRoles()
        {
            return View(roleManager.Roles);
        }

        [HttpGet]
        [Authorize(Policy = "EditPolicy")]
        [Route("edit-role/{id}")]
        public async Task<IActionResult> EditRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return RoleNotFound(id);
            }

            EditRoleViewModel model = new EditRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name
            };

            IList<ApplicationUser> users = await userManager.GetUsersInRoleAsync(role.Name);
            model.UserNames = users.Select(user => user.UserName);

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditPolicy")]
        [Route("edit-role/{id?}")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            IdentityRole role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                return RoleNotFound(model.Id);
            }

            role.Name = model.RoleName;

            IdentityResult result = await roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "DeletePolicy")]
        [Route("delete-role/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return RoleNotFound(id);
            }

            try
            {
                IdentityResult result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("ListRoles");
            }
            catch (DbUpdateException)
            {
                var model = new ExceptionDetailsViewModel()
                {
                    Title = $"{role.Name} role is currrently in use",
                    Message = $"First delete all the users from {role.Name} role and then try to delete it."
                };

                return View("Error", model);
            }
        }

        [HttpGet]
        [Authorize(Policy = "EditPolicy")]
        [Route("edit-users-in-role/{roleId}")]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            IdentityRole role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return RoleNotFound(roleId);
            }

            var model = new EditUsersInRoleViewModel()
            {
                RoleId = role.Id
            };

            List<ApplicationUser> users = await userManager.Users.ToListAsync();

            foreach (ApplicationUser user in users)
            {
                var userInRole = new UserInRoleViewModel
                {
                    Id = user.Id,
                    Name = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userInRole.IsRoleSelected = true;
                }

                model.Users.Add(userInRole);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditPolicy")]
        [Route("edit-users-in-role/{id?}")]
        public async Task<IActionResult> EditUsersInRole(EditUsersInRoleViewModel model)
        {
            IdentityRole role = await roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
            {
                return RoleNotFound(model.RoleId);
            }

            for (int i = 0; i < model.Users.Count; i++)
            {
                UserInRoleViewModel currentUser = model.Users[i];
                ApplicationUser user = await userManager.FindByIdAsync(currentUser.Id);
                bool isCurrentlyInRole = await userManager.IsInRoleAsync(user, role.Name);

                if (currentUser.IsRoleSelected && !isCurrentlyInRole)
                {
                    await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!currentUser.IsRoleSelected && isCurrentlyInRole)
                {
                    await userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            return RedirectToAction("editrole", new { role.Id });
        }

        private ViewResult UserNotFoundView(string id)
        {
            ViewBag.ErrorMessage = $"User with ID {id} is not found.";
            return View("NotFound");
        }

        private ViewResult RoleNotFound(string id)
        {
            ViewBag.ErrorMessage = $"Role with ID {id} is not found.";
            return View("NotFound");
        }
    }
}
