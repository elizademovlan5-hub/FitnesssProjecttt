using FitnessProjecttt.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessProjecttt.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Workout> Workouts { get; set; }

    public DbSet<EmailVerification> EmailVerifications { get; set; }

    public DbSet<WeeklyPlan> WeeklyPlans { get; set; }

    public DbSet<CompletedWorkout> CompletedWorkouts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>()
            .HasIndex(x => x.Slug)
            .IsUnique();

        builder.Entity<Workout>()
            .HasIndex(x => x.Slug)
            .IsUnique();

        builder.Entity<Category>()
            .HasMany(x => x.Workouts)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<WeeklyPlan>()
            .HasOne(x => x.Workout)
            .WithMany()
            .HasForeignKey(x => x.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CompletedWorkout>()
            .HasOne(x => x.Workout)
            .WithMany()
            .HasForeignKey(x => x.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}