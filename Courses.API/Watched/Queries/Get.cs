using Courses.API.Database;
using Courses.Shared;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Watched.Queries;

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

        if (watchedDuration > 0)
        {
            var course = await _context.Courses.FirstAsync(c => c.Id == request.CourseId, cancellationToken).ConfigureAwait(false);
            progress = decimal.Divide(watchedDuration, course.Duration.Ticks) * 100;
        }

        var @return = new GetWatchedResponse(watched.Length, TimeSpan.FromTicks(watchedDuration).ToString("h'h 'm'm'"), progress,
            watched.Adapt<GetWatchedResponseEntryView[]>());
        return @return;
    }
}