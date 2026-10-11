
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
public class WeeklyPlanController : ControllerBase
{
    private readonly AppDbContext _context;

    public WeeklyPlanController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyPlan()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var plan = await _context.WeeklyPlans
            .Include(x => x.Workout)
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                x.PlannedDate,
                Workout = new
                {
                    x.Workout!.Name,
                    x.Workout.Slug,
                    x.Workout.ImageUrl,
                    x.Workout.Difficulty
                }
            })
            .ToListAsync();

        return Ok(plan);
    }

    [HttpPost("{workoutSlug}")]
    public async Task<IActionResult> Add(
        string workoutSlug,
        DateTime plannedDate)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var workout = await _context.Workouts
            .FirstOrDefaultAsync(x =>
                x.Slug == workoutSlug);

        if (workout == null)
            return NotFound("Workout tapilmadi.");

        var plan = new WeeklyPlan
        {
            UserId = userId!,
            WorkoutId = workout.Id,
            PlannedDate = plannedDate
        };

        _context.WeeklyPlans.Add(plan);

        await _context.SaveChangesAsync();

        return Ok("Workout hefdelik plana elave edildi.");
    }

    [HttpDelete("{workoutSlug}")]
    public async Task<IActionResult> Remove(
        string workoutSlug,
        DateTime plannedDate)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var workout = await _context.Workouts
            .FirstOrDefaultAsync(x =>
                x.Slug == workoutSlug);

        if (workout == null)
            return NotFound("Workout tapilmadi.");

        var plan = await _context.WeeklyPlans
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.WorkoutId == workout.Id &&
                x.PlannedDate.Date == plannedDate.Date);

        if (plan == null)
            return NotFound("Plan tapilmadi.");

        _context.WeeklyPlans.Remove(plan);

        await _context.SaveChangesAsync();

        return Ok("Workout plandan silindi.");
    }
}