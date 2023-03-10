namespace WatchModule.Entities;

public class Watch
{
    public int Id { get; init; }

    public int CourseId { get; init; }

    public int EntryId { get; init; }

    public DateTimeOffset CreatedAt { get; set; }
}