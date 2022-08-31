using System.ComponentModel.DataAnnotations;

namespace Courses.API.Courses;

public class Course
{
    public int Id { get; set; }

    [StringLength(128)] public string Name { get; set; } = null!;

    public TimeSpan Duration { get; set; }

    public string[]? Categories { get; set; } = Array.Empty<string>();

    public bool IsHighDefinition { get; set; }

    [StringLength(512)] public string Path { get; set; } = null!;

    [StringLength(64)] public string Host { get; set; } = null!;

    //# Entity Framework
    public Author? Author { get; set; } = null!;

    public Platform? Platform { get; set; } = null!;

    public List<CourseEntry> Entries { get; set; } = new();
}