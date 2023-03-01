namespace SharedModule;

public static class Functions
{
    public static string FormatDuration(TimeSpan duration)
    {
        return duration.TotalHours switch
        {
            > 24 => $"{Math.Round(duration.TotalHours)}h",
            >= 1 => duration.ToString("h'h 'm'm'"),
            _ => "h'h 'm'm'"
        };
    }
}