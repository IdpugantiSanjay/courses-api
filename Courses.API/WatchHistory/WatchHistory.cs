using System.ComponentModel.DataAnnotations.Schema;
using Courses.API.Courses;

namespace Courses.API.WatchHistory;

public class WatchHistory
{
    public int Id { get; set; }

    [ForeignKey(nameof(Course))] public int CourseId { get; set; }

    [ForeignKey(nameof(CourseEntry))] public int EntryId { get; set; }

    public CourseEntry Entry { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual Course Course { get; set; }
}