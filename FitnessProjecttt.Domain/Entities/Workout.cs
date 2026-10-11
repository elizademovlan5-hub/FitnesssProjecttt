using FitnessProjecttt.Domain.Entities;

namespace FitnessProjecttt.Domain.Entities;

public class Workout
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string MuscleGroup { get; set; } = string.Empty;

    public string Difficulty { get; set; } = string.Empty;

    public int Sets { get; set; }

    public int Repetitions { get; set; }

    public int Duration { get; set; }

    public string? ImageUrl { get; set; }

    public string? VideoUrl { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}