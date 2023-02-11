using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.API.Courses;

[Table("Tag")]
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public List<Course> Courses { get; set; } = null!;
}