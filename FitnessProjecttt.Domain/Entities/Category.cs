using FitnessProjecttt.Domain.Entities;

namespace FitnessProjecttt.Domain.Entities;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}