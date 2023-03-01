namespace Courses.API.Database;
//
// public abstract class AppDbContext : DbContext
// {
//     protected abstract string Schema { get; }
//
//     protected AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
//     {
//     }
//
//     public DbSet<Course> Courses { get; set; } = null!;
//
//     public DbSet<WatchHistory.WatchHistory> Watched { get; set; } = null!;
//
//     public DbSet<CourseEntry> CourseEntries { get; set; } = null!;
//
//     public DbSet<Platform> Platforms { get; set; } = null!;
//
//     public DbSet<Author> Authors { get; set; } = null!;
//
//     public DbSet<Notes.Notes> Notes { get; set; } = null!;
//
//     protected override void OnModelCreating(ModelBuilder builder)
//     {
//         builder.HasDefaultSchema(Schema);
//
//         
//         builder.Entity<Notes.Notes>().Property(c => c.Note).HasMaxLength(4096);
//
//         builder.Entity<Notes.Notes>().HasIndex(x => new { x.EntryId, x.CourseId }).IsUnique();
//         
//         base.OnModelCreating(builder);
//     }
// }