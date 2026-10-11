namespace FitnessProjecttt.Application.DTOs.Workout;

public class CreateWorkoutDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string MuscleGroup { get; set; } = string.Empty;

    public string Difficulty { get; set; } = string.Empty;

    public int Sets { get; set; }

    public int Repetitions { get; set; }

    public int Duration { get; set; }

    public int CategoryId { get; set; }
}