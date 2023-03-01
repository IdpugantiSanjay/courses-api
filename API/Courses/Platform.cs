using System.ComponentModel.DataAnnotations;

namespace Courses.API.Courses;

public class Platform
{
    public int Id { get; set; }

    [StringLength(64)] public string Name { get; set; } = null!;
}