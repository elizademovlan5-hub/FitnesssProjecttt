using FitnessProjecttt.Domain.Entities;
using FitnessProjecttt.Application.DTOs.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessProjecttt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public ProfileController(
        UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        return Ok(new
        {
            EmailFromToken = email
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        UpdateProfileDto dto)
    {
        var email = User.FindFirstValue(
            ClaimTypes.Email);

        var user = await _userManager
            .FindByEmailAsync(email!);

        if (user == null)
            return NotFound();

        user.FullName = dto.FullName;

        await _userManager.UpdateAsync(user);

        return Ok("Profil yenilendi.");
    }
}