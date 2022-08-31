using System.ComponentModel.DataAnnotations;

namespace Courses.API.Courses;

public class Author
{
    public int Id { get; set; }

    [StringLength(64)] public string Name { get; set; } = null!;
}