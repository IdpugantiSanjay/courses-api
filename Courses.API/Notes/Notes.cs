using Courses.API.Courses;

namespace Courses.API.Notes;

public class Notes
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public int EntryId { get; set; }
    public string Note { get; set; } = string.Empty;

    public Course Course { get; set; }

    public CourseEntry Entry { get; set; }
}