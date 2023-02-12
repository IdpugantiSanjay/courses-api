using JetBrains.Annotations;
using MediatR;

namespace Courses.Shared;

// public record CourseDto(int Id, string Name, TimeSpan Duration, string[]? Categories, bool IsHighDefinition, string? Author, string? Platform, string Path, string Host, CourseEntryDto[] Entries);
// public record CourseEntryDto(int Id, string Name, TimeSpan Duration, int SequenceNumber, string? Section);

public record CreateCourseRequest(string Name, TimeSpan Duration, string[]? Categories, bool IsHighDefinition,
    string? Author, string? Platform, string Path, string Host, CreateCourseRequestEntry[] Entries) : IRequest;

public record CreateCourseFromPlaylistRequest(string Name, TimeSpan Duration, string[]? Categories, bool IsHighDefinition, string PlaylistId,
    CreateCourseFromPlaylistRequestEntry[] Entries) : IRequest;

public record CreateCourseRequestEntry(string Name, TimeSpan Duration, int SequenceNumber, string? Section);

public record CreateCourseFromPlaylistRequestEntry(string Name, TimeSpan Duration, int SequenceNumber, string VideoId);

public record GetCoursesRequest : IRequest<GetCoursesResponse>, IRequest<Unit>;

public record GetCoursesResponse(GetCourseView[] Courses);

[UsedImplicitly]
public record GetCourseView(int Id, string Name, string Duration, string[]? Categories, bool IsHighDefinition, decimal Progress, string? PlaylistId);

// public record ProgressInfo(int WatchedCount, string WatchedDuration, decimal Progress);

public record GetCourseByIdRequest(int Id) : IRequest<GetByIdCourseView>;

public record DeleteCourseByIdRequest(int Id) : IRequest;

[UsedImplicitly]
public record GetByIdCourseView(int Id, string Name, string Duration, string[]? Categories, bool IsHighDefinition,
    string? Author, string? Platform, string? Path, string? Host, string? PlaylistId, GetByIdCourseEntryView[] Entries);

[UsedImplicitly]
public record GetByIdCourseEntryView(int Id, string Name, string Duration, int SequenceNumber, string? Section, bool HasNotes, string? VideoId);

[UsedImplicitly]
public record GetWatchedRequest(int CourseId) : IRequest<GetWatchedResponse>;

[UsedImplicitly]
public record GetWatchedResponse(int WatchedCount, string WatchedDuration, decimal Progress, GetWatchedResponseEntryView[] WatchedEntries);

[UsedImplicitly]
public record GetWatchedResponseEntryView(int Id);

public record SetWatchedRequest(int CourseId, int CourseEntryId) : IRequest<Unit>;

public record DeletedWatchedRequest(int CourseId, int CourseEntryId) : IRequest<Unit>;