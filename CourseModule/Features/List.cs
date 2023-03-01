using System.ComponentModel;
using Contracts;
using CourseModule.Contracts;
using Injectio.Attributes;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace CourseModule.Features;

public record ListCoursesRequest : IListRequest<int, CourseView>
{
    public required int ParentId { get; init; }
    public int PageSize { get; init; } = 10;
    public string PageToken { get; init; } = string.Empty;
    public CourseView View { get; init; } = CourseView.Default;
}

public record ListCoursesResponse<TResponse>(TResponse[] Items, string NextPageToken) : IListResponse<TResponse> where TResponse : CourseResponse;

[RegisterScoped<IList<int, CourseView, CourseResponse>>]
public sealed partial class CourseService : IList<int, CourseView, CourseResponse>
{
    public async Task<OneOf<IListResponse<CourseResponse>, Error<Exception>>> List(IListRequest<int, CourseView> request, CancellationToken cancellationToken)
    {
        switch (request.View)
        {
            case CourseView.Default:
            {
                var projection = _context.Courses.ProjectToType<CourseResponse.Default>();
                var items = await projection.ToArrayAsync(cancellationToken);
                return new ListCoursesResponse<CourseResponse.Default>(items, string.Empty);
            }
            case CourseView.Entries:
            {
                var projection = _context.Courses.ProjectToType<CourseResponse.WithEntries>();
                var items = await projection.ToArrayAsync(cancellationToken);
                return new ListCoursesResponse<CourseResponse.WithEntries>(items, string.Empty);
            }
            default:
                return new Error<Exception>(new InvalidEnumArgumentException($"""
                Invalid enum value. Or enum value {request.View} not handled
                """));
        }
    }
}