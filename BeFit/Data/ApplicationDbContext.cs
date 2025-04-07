using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ExerciseType> ExerciseTypes { get; set; }
    public DbSet<WorkoutSession> WorkoutSessions { get; set; }
    public DbSet<PerformedExercise> PerformedExercises { get; set; }
}