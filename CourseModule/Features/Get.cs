using System.ComponentModel;
using Contracts;
using CourseModule.Contracts;
using Injectio.Attributes;
using Mapster;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace CourseModule.Features;

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

public sealed class Handler : IRequestHandler<GetCourseRequest, OneOf<CourseResponse, NotFound, Error<Exception>>>
{
    private readonly IGet<int, int, CourseView, CourseResponse> _service;

    public Handler(IGet<int, int, CourseView, CourseResponse> service)
    {
        _service = service;
    }

    public async ValueTask<OneOf<CourseResponse, NotFound, Error<Exception>>> Handle(GetCourseRequest request, CancellationToken cancellationToken)
    {
        return await _service.Get(request, cancellationToken);
    }
}