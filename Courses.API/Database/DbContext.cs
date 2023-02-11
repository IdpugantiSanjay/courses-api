using Courses.API.Courses;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; } = null!;

    public DbSet<WatchHistory.WatchHistory> Watched { get; set; } = null!;

    public DbSet<CourseEntry> CourseEntries { get; set; } = null!;

    public DbSet<Platform> Platforms { get; set; } = null!;

    public DbSet<Author> Authors { get; set; } = null!;

    public DbSet<Notes.Notes> Notes { get; set; } = null!;

    public DbSet<Tag> Tags { get; set; } = null!;

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
        builder.Entity<Notes.Notes>().Property(c => c.Note).HasMaxLength(4096);

        builder.Entity<Notes.Notes>().HasIndex(x => new { x.EntryId, x.CourseId }).IsUnique();

        var id = 1;
        builder.Entity<Tag>().HasData(new List<Tag>
        {
            new() { Id = id++, Name = ".NET" },
            new() { Id = id++, Name = "JavaScript" },
            new() { Id = id++, Name = "Web" },
            new() { Id = id++, Name = "CSS" },
            new() { Id = id++, Name = "Database" },
            new() { Id = id++, Name = "AWS" },
            new() { Id = id++, Name = "Security" },
            new() { Id = id++, Name = "Git" },
            new() { Id = id++, Name = "GitHub" },
            new() { Id = id, Name = "Docker" }
        });
    }
}