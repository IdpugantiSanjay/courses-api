using JetBrains.Annotations;
using MediatR;

namespace Courses.Shared;

// public record CourseDto(int Id, string Name, TimeSpan Duration, string[]? Categories, bool IsHighDefinition, string? Author, string? Platform, string Path, string Host, CourseEntryDto[] Entries);
// public record CourseEntryDto(int Id, string Name, TimeSpan Duration, int SequenceNumber, string? Section);

public record CreateCourseRequest(string Name, TimeSpan Duration, string[]? Categories, bool IsHighDefinition,
    string? Author, string? Platform, string Path, string Host, CreateCourseRequestEntry[] Entries) : IRequest;

public record CreateCourseRequestEntry(string Name, TimeSpan Duration, int SequenceNumber, string? Section);

public record GetCoursesRequest : IRequest<GetCoursesResponse>, IRequest<Unit>;

public record GetCoursesResponse(GetCourseView[] Courses);

[UsedImplicitly]
public record GetCourseView(int Id, string Name, TimeSpan Duration, string[]? Categories, bool IsHighDefinition,
    string? Author, string? Platform);

public record GetCourseByIdRequest(int Id) : IRequest<GetByIdCourseView>;

public record DeleteCourseByIdRequest(int Id) : IRequest;

[UsedImplicitly]
public record GetByIdCourseView(int Id, string Name, TimeSpan Duration, string[]? Categories, bool IsHighDefinition,
    string? Author, string? Platform, string Path, string Host, GetByIdCourseEntryView[] Entries);

[UsedImplicitly]
public record GetByIdCourseEntryView(int Id, string Name, TimeSpan Duration, int SequenceNumber, string? Section);