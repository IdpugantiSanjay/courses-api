using System.ComponentModel;
using Contracts;
using CourseModule.Contracts;
using Injectio.Attributes;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace CourseModule.Features;

public record GetCourseRequest(int ParentId, int Id, CourseView View) : IGetRequest<int, int, CourseView>;

[RegisterScoped<IGet<int, int, CourseView, CourseResponse>>]
public sealed partial class CourseService : IGet<int, int, CourseView, CourseResponse>
{
    public async Task<OneOf<CourseResponse, NotFound, Error<Exception>>> Get(IGetRequest<int, int, CourseView> request,
        CancellationToken cancellationToken)
    {
        var query = _context.Courses.Where(c => c.Id == request.Id);
        IQueryable<CourseResponse> projection;
        switch (request.View)
        {
            case CourseView.Default:
                projection = query.ProjectToType<CourseResponse.Default>();
                break;
            case CourseView.Entries:
                projection = query.Include(q => q.Entries).ProjectToType<CourseResponse.WithEntries>();
                break;
            default:
                return new Error<Exception>(new InvalidEnumArgumentException($"""
                Invalid enum value. Or enum value {request.View} not handled
                """));
        }

        var entity = await projection.FirstOrDefaultAsync(cancellationToken);
        return entity is null ? new NotFound() : entity;
    }
}