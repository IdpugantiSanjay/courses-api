using Courses.API.Database;
using Courses.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Courses.API.Courses.Functions;

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
        var courses =
            await _context.Courses
                .Include(c => c.WatchHistory)
                .ThenInclude(wh => wh.Entry)
                .ToArrayAsync(cancellationToken);
        ;

        var courseViews = from course in courses
            let watchedDuration = course.WatchHistory.Select(w => w.Entry).Sum(x => x.Duration.Ticks)
            let progress = decimal.Divide(watchedDuration, course.Duration.Ticks) * 100
            select new GetCourseView(course.Id, course.Name, FormatDuration(course.Duration), Array.Empty<string>(), course.IsHighDefinition, progress,
                course.PlaylistId);

        return new GetCoursesResponse(courseViews.ToArray());
    }
}