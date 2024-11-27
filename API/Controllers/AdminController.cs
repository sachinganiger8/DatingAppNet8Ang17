using System;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [HttpGet("users-with-roles")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult> GetUsersWithRole()
    {
        var usersWithRoles = await userManager.Users.Select(s => new
        {
            s.Id,
            Username = s.UserName,
            Roles = s.UserRoles.Select(r => r.Role.Name).ToList()
        }).ToListAsync();

        return Ok(usersWithRoles);
    }

    [HttpPost("edit-roles/{username}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select atleast one role");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed remove roles");

        return Ok(await userManager.GetRolesAsync(user));
    }

    [HttpGet("photos-to-moderate")]
    [Authorize(Policy = "ModeratePhotoRole")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admin or Moderator can see this");
    }
}
