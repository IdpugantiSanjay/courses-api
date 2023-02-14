using Courses.API.Database;
using Courses.Shared;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Courses.API.Courses.Functions;

namespace Courses.API.WatchHistory.Queries;

public class Get : IRequestHandler<GetWatchedRequest, GetWatchedResponse>
{
    private readonly AppDbContext _context;

    public Get(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetWatchedResponse> Handle(GetWatchedRequest request, CancellationToken cancellationToken)
    {
        var watched = await _context.Watched.Where(w => w.CourseId == request.CourseId).Include(w => w.Entry)
            .ToArrayAsync(cancellationToken);

        var watchedDuration = watched.Select(w => w.Entry).Sum(x => x.Duration.Ticks);
        decimal progress = 0;
        var durationLeft = string.Empty;

        if (watchedDuration > 0)
        {
            var course = await _context.Courses.Where(c => c.Id == request.CourseId).FirstAsync(cancellationToken);
            progress = decimal.Divide(watchedDuration, course.Duration.Ticks) * 100;
            durationLeft = FormatDuration(course.Duration - watched.Select(w => w.Entry.Duration).Aggregate(TimeSpan.Zero, (acc, curr) => acc + curr));
        }

        var @return = new GetWatchedResponse(watched.Length, TimeSpan.FromTicks(watchedDuration).ToString("h'h 'm'm'"), progress, durationLeft,
            watched.Adapt<GetWatchedResponseEntryView[]>());
        return @return;
    }
}