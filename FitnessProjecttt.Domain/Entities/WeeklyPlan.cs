using FitnessProjecttt.Domain.Entities;

namespace FitnessProjecttt.Domain.Entities;

public class WeeklyPlan
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int WorkoutId { get; set; }

    public DateTime PlannedDate { get; set; }

    public Workout? Workout { get; set; }
}