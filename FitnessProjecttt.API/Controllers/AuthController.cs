
using FitnessProjecttt.Application.DTOs.Auth;
using FitnessProjecttt.Application.Interfaces;
using FitnessProjecttt.Domain.Entities;
using FitnessProjecttt.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessProjecttt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly AppDbContext _context;

    public AuthController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtService jwtService,
        IEmailService emailService,
        AppDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _emailService = emailService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
            return BadRequest("Bu email artiq qeydiyatdan kecib.");

        var oldVerification = await _context.EmailVerifications
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (oldVerification != null)
        {
            _context.EmailVerifications.Remove(oldVerification);
            await _context.SaveChangesAsync();
        }

        var passwordHasher = new PasswordHasher<User>();

        var tempUser = new User
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var passwordHash = passwordHasher.HashPassword(
            tempUser,
            dto.Password);

        var otp = Random.Shared
            .Next(100000, 999999)
            .ToString();

        var verification = new EmailVerification
        {
            Email = dto.Email,
            FullName = dto.FullName,
            PasswordHash = passwordHash,
            Otp = otp,
            ExpirationTime = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        _context.EmailVerifications.Add(verification);

        await _context.SaveChangesAsync();

        await _emailService.SendOtpAsync(
            dto.Email,
            otp);

        return Ok("Otp emaile gonderildi.");
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(
        VerifyOtpDto dto)
    {
        var verification = await _context.EmailVerifications
            .FirstOrDefaultAsync(x =>
                x.Email == dto.Email &&
                x.Otp == dto.Otp &&
                !x.IsUsed);

        if (verification == null)
            return BadRequest("Otp yanlisdir.");

        if (verification.ExpirationTime < DateTime.UtcNow)
            return BadRequest("Otp vaxti bitib.");

        var user = new User
        {
            UserName = verification.Email,
            Email = verification.Email,
            FullName = verification.FullName,
            EmailConfirmed = true
        };

        user.PasswordHash = verification.PasswordHash;

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(
                new IdentityRole("User"));
        }

        await _userManager.AddToRoleAsync(
            user,
            "User");

        verification.IsUsed = true;

        await _context.SaveChangesAsync();

        return Ok("Email tesdiglendi qeydiyatdan kecirildi.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager
            .FindByEmailAsync(dto.Email);

        if (user == null)
            return Unauthorized("Email ve sifre yanlisdir.");

        if (!user.EmailConfirmed)
            return Unauthorized(
                "Evvelce email tesdiglenmelidir.");

        var passwordValid =
            await _userManager.CheckPasswordAsync(
                user,
                dto.Password);

        if (!passwordValid)
            return Unauthorized(
                "Email ve sifre yanlisdir.");

        var roles = await _userManager.GetRolesAsync(user);

        var role = roles.FirstOrDefault() ?? "User";

        var token = _jwtService.GenerateToken(
            user.Id,
            user.Email!,
            user.FullName,
            role);

        return Ok(new
        {
            token,
            role,
            fullName = user.FullName
        });
    }
}