using FitnessProjecttt.Domain.Entities;
using FitnessProjecttt.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessProjecttt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatisticsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("complete/{workoutSlug}")]
    public async Task<IActionResult> Complete(
        string workoutSlug)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var workout = await _context.Workouts
            .FirstOrDefaultAsync(x =>
                x.Slug == workoutSlug);

        if (workout == null)
            return NotFound("Workout tapilmadi.");

        var completed = new CompletedWorkout
        {
            UserId = userId!,
            WorkoutId = workout.Id,
            CompletedAt = DateTime.UtcNow
        };

        _context.CompletedWorkouts.Add(completed);

        await _context.SaveChangesAsync();

        return Ok("Workout tamamlandi.");
    }

    [HttpGet("today")]
    public async Task<IActionResult> Today()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var today = DateTime.UtcNow.Date;

        var count = await _context.CompletedWorkouts
            .CountAsync(x =>
                x.UserId == userId &&
                x.CompletedAt.Date == today);

        return Ok(new
        {
            completedToday = count
        });
    }

    [HttpGet("week")]
    public async Task<IActionResult> Week()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var startDate = DateTime.UtcNow.Date
            .AddDays(-(int)DateTime.UtcNow.DayOfWeek);

        var count = await _context.CompletedWorkouts
            .CountAsync(x =>
                x.UserId == userId &&
                x.CompletedAt >= startDate);

        return Ok(new
        {
            completedThisWeek = count
        });
    }

    [HttpGet("history")]
    public async Task<IActionResult> History()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var history = await _context.CompletedWorkouts
            .Include(x => x.Workout)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CompletedAt)
            .Select(x => new
            {
                Workout = x.Workout!.Name,
                Slug = x.Workout.Slug,
                x.CompletedAt
            })
            .ToListAsync();

        return Ok(history);
    }
}