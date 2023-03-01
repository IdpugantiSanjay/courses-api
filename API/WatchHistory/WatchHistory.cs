using System.ComponentModel.DataAnnotations.Schema;
using Courses.API.Courses;
using JetBrains.Annotations;

namespace Courses.API.WatchHistory;

[UsedImplicitly]
public class WatchHistory
{
    public int Id { get; set; }

    [ForeignKey(nameof(Course))] public int CourseId { get; set; }

    [ForeignKey(nameof(CourseEntry))] public int EntryId { get; set; }

    public CourseEntry Entry { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public virtual Course Course { get; init; } = null!;
}