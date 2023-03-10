using Microsoft.EntityFrameworkCore;
using SharedModule;
using WatchModule.Entities;

namespace WatchModule.Database;

public class WatchDbContext : DbContext, IAppDbContext
{
    public WatchDbContext(DbContextOptions<WatchDbContext> options) : base(options)
    {
    }

    public DbSet<Watch> WatchHistory { get; set; } = null!;

    public static string Schema => "watch";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Watch>().HasIndex(x => new { x.CourseId, x.EntryId }).IsUnique();
    }
}