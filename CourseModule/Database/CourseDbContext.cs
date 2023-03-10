using CourseModule.Entities;
using Microsoft.EntityFrameworkCore;
using SharedModule;

namespace CourseModule.Database;

public class CourseDbContext : DbContext, IAppDbContext
{
    public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
    {
    }

    public required DbSet<Course> Courses { get; init; } = null!;

    public static string Schema => "course";

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Course>()
            .Property(c => c.Duration)
            .HasConversion<long>()
            ;

        builder.Entity<Course>()
            .HasIndex(c => c.Name)
            .IsUnique()
            ;

        builder.Entity<CourseEntry>().Property(c => c.Duration).HasConversion<long>();

        base.OnModelCreating(builder);
    }
}