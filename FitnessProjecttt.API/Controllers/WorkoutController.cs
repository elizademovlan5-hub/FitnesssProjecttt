using FitnessProjecttt.Application.DTOs.Workout;
using FitnessProjecttt.Application.Interfaces;
using FitnessProjecttt.Domain.Entities;
using FitnessProjecttt.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessProjecttt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ICloudinaryService _cloudinary;

    public WorkoutController(
        AppDbContext context,
        ICloudinaryService cloudinary)
    {
        _context = context;
        _cloudinary = cloudinary;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        string? search,
        string? category,
        string? difficulty)
    {
        var query = _context.Workouts
            .Include(x => x.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Description.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x =>
                x.Category!.Slug == category);
        }

        if (!string.IsNullOrWhiteSpace(difficulty))
        {
            query = query.Where(x =>
                x.Difficulty == difficulty);
        }

        var workouts = await query
            .Select(x => new
            {
                x.Name,
                x.Slug,
                x.Description,
                x.MuscleGroup,
                x.Difficulty,
                x.Sets,
                x.Repetitions,
                x.Duration,
                x.ImageUrl,
                x.VideoUrl,
                Category = x.Category!.Name
            })
            .ToListAsync();

        return Ok(workouts);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(
        string slug)
    {
        var workout = await _context.Workouts
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Slug == slug);

        if (workout == null)
            return NotFound("Workout tapilmadi.");

        return Ok(workout);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] CreateWorkoutDto dto,
        IFormFile? image,
        IFormFile? video)
    {
        var category = await _context.Categories
            .FindAsync(dto.CategoryId);

        if (category == null)
            return BadRequest("Category tapilmadi.");

        var slug = CreateSlug(dto.Name);

        if (await _context.Workouts
            .AnyAsync(x => x.Slug == slug))
        {
            return BadRequest(
                "Bu adda workout artiq movcuddur.");
        }

        string? imageUrl = null;
        string? videoUrl = null;

        if (image != null)
        {
            imageUrl =
                await _cloudinary.UploadImageAsync(image);
        }

        if (video != null)
        {
            videoUrl =
                await _cloudinary.UploadVideoAsync(video);
        }

        var workout = new Workout
        {
            Name = dto.Name,
            Slug = slug,
            Description = dto.Description,
            MuscleGroup = dto.MuscleGroup,
            Difficulty = dto.Difficulty,
            Sets = dto.Sets,
            Repetitions = dto.Repetitions,
            Duration = dto.Duration,
            CategoryId = dto.CategoryId,
            ImageUrl = imageUrl,
            VideoUrl = videoUrl
        };

        _context.Workouts.Add(workout);

        await _context.SaveChangesAsync();

        return Ok(workout);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{slug}")]
    public async Task<IActionResult> Update(
        string slug,
        [FromForm] CreateWorkoutDto dto,
        IFormFile? image,
        IFormFile? video)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(x => x.Slug == slug);

        if (workout == null)
            return NotFound("Workout tapilmadi.");

        var category = await _context.Categories
            .FindAsync(dto.CategoryId);

        if (category == null)
            return BadRequest("Category tapilmadi.");

        workout.Name = dto.Name;
        workout.Slug = CreateSlug(dto.Name);
        workout.Description = dto.Description;
        workout.MuscleGroup = dto.MuscleGroup;
        workout.Difficulty = dto.Difficulty;
        workout.Sets = dto.Sets;
        workout.Repetitions = dto.Repetitions;
        workout.Duration = dto.Duration;
        workout.CategoryId = dto.CategoryId;

        if (image != null)
        {
            workout.ImageUrl =
                await _cloudinary.UploadImageAsync(image);
        }

        if (video != null)
        {
            workout.VideoUrl =
                await _cloudinary.UploadVideoAsync(video);
        }

        await _context.SaveChangesAsync();

        return Ok(workout);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{slug}")]
    public async Task<IActionResult> Delete(
        string slug)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(x => x.Slug == slug);

        if (workout == null)
            return NotFound("Workout tapilmadi.");

        _context.Workouts.Remove(workout);

        await _context.SaveChangesAsync();

        return Ok("Workout silindi.");
    }

    private string CreateSlug(string text)
    {
        return text
            .Trim()
            .ToLower()
            .Replace(" ", "-");
    }
}