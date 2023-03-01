using System.ComponentModel.DataAnnotations;

namespace CourseModule.Entities;

public class CourseEntry
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public string? VideoId { get; set; }

    public int SequenceNumber { get; set; }

    [StringLength(128)] public string Name { get; set; } = null!;
    public TimeSpan Duration { get; set; }

    [StringLength(128)] public string? Section { get; set; }

    // public Notes.Notes? Notes { get; set; }
}