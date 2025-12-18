using ControlSpark.WebMvc.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using InquirySpark.Admin.Models;

namespace InquirySpark.Admin.Controllers;

public class RoleManagementController(
    UserManager<ControlSparkUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<RoleManagementController> logger) : BaseController(logger)
{
    private readonly UserManager<ControlSparkUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var userRolesViewModel = new List<UserRolesViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRolesViewModel.Add(new UserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                Roles = roles.ToList(),
                EmailConfirmed = user.EmailConfirmed
            });
        }

        return View(userRolesViewModel);
    }

    public async Task<IActionResult> Manage(string userId)
    {
        ViewBag.UserId = userId;
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
            return View("NotFound");
        }

        ViewBag.UserName = user.UserName;
        var model = new List<ManageUserRolesViewModel>();
        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in _roleManager.Roles)
        {
            var userRolesViewModel = new ManageUserRolesViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name ?? ""
            };

            userRolesViewModel.Selected = userRoles.Contains(role.Name ?? "");
            model.Add(userRolesViewModel);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return View("NotFound");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, roles);
        
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot remove user existing roles");
            return View(model);
        }

        result = await _userManager.AddToRolesAsync(user, 
            model.Where(x => x.Selected).Select(y => y.RoleName));
        
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot add selected roles to user");
            return View(model);
        }

        _logger.LogInformation("Roles updated for user {UserName} by {CurrentUser}", 
            user.UserName, User.Identity?.Name);

        return RedirectToAction("Index");
    }
}
