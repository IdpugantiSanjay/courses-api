namespace Courses.API.Courses;

internal static class Functions
{
    public static string FormatDuration(TimeSpan duration)
    {
        return duration.TotalHours > 24 ? $"{Math.Round(duration.TotalHours)}h" : duration.ToString(duration.TotalHours >= 1 ? "h'h 'm'm'" : "m'm'");
    }
}