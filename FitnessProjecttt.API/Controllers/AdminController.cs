
using FitnessProjecttt.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessProjecttt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AdminController(
        UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _userManager.Users
            .Select(x => new
            {
                x.Id,
                x.FullName,
                x.Email
            })
            .ToList();

        return Ok(users);
    }

    [HttpPut("users/{email}/role")]
    public async Task<IActionResult> ChangeRole(
        string email,
        string role)
    {
        if (role != "Admin" && role != "User")
            return BadRequest(
                "Rol yalniz Admin ve User ucun ola biler.");

        var user = await _userManager
            .FindByEmailAsync(email);

        if (user == null)
            return NotFound("User tapilmadi.");

        var currentRoles =
            await _userManager.GetRolesAsync(user);

        if (currentRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(
                user,
                currentRoles);
        }

        await _userManager.AddToRoleAsync(
            user,
            role);

        return Ok("User Rol yenilendi.");
    }
}