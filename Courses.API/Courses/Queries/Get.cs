using Courses.API.Database;
using Courses.Shared;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Courses.Queries;

public class GetCoursesHandler : IRequestHandler<GetCoursesRequest, GetCoursesResponse>
{
    private readonly AppDbContext _context;

    public GetCoursesHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetCoursesResponse> Handle(GetCoursesRequest request, CancellationToken cancellationToken)
    {
        var query =
                _context.Courses
            // .Include(c => c.Author)
            // .Include(c => c.Platform)
            ;

        var courses = await query.ProjectToType<GetCourseView>().ToArrayAsync(cancellationToken);
        return new GetCoursesResponse(courses);
    }
}