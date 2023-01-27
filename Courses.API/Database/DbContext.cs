using Courses.API.Courses;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; } = null!;

    public DbSet<Watched.Watched> Watched { get; set; } = null!;

    public DbSet<CourseEntry> CourseEntries { get; set; } = null!;

    public DbSet<Platform> Platforms { get; set; } = null!;

    public DbSet<Author> Authors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Course>()
            .Property(c => c.Duration)
            .HasConversion<long>()
            ;

        builder.Entity<Course>()
            .HasIndex(c => c.Name)
            .IsUnique()
            ;

        builder.Entity<CourseEntry>().Property(c => c.Duration).HasConversion<long>();

        // builder.Entity<Watched.Watched>().HasNoKey();
    }
}