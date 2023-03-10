using Contracts;
using CourseModule.Contracts;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using WatchModule.Contracts;
using static SharedModule.Functions;

namespace WatchModule.Features;

using WatchServiceGetResponse = OneOf<WatchStatsResponse, NotFound, Error<Exception>>;

public record GetWatchRequest(int ParentId, int Id, CourseWatchStatsView View) : IGetRequest<int, int, CourseWatchStatsView>;

// [RegisterScoped<IGet<int, int, CourseWatchStatsView, WatchStatsResponse>>]
public partial class WatchService : IGet<int, int, CourseWatchStatsView, WatchStatsResponse>
{
    public async Task<WatchServiceGetResponse> Get(IGetRequest<int, int, CourseWatchStatsView> request, CancellationToken cancellationToken)
    {
        try
        {
            var courseId = request.Id;
            var courseResponse = await _mediator.Send(new GetCourseRequest(default, courseId, CourseView.Entries), cancellationToken);
            var watchedIdList = await _context.WatchHistory.Where(c => c.CourseId == request.Id).Select(c => c.EntryId)
                .ToArrayAsync(cancellationToken);
            var watchedIdSet = new HashSet<int>(watchedIdList);

            return courseResponse.Match<WatchServiceGetResponse>(
                course => GetWatchStatsResponse((course as CourseResponse.WithEntries)!),
                notFound => notFound,
                error => error
            );

            WatchStatsResponse GetWatchStatsResponse(CourseResponse.WithEntries course)
            {
                var watchedTimeSpan = TimeSpan.Zero;
                var timeSpanLeftToWatch = TimeSpan.Zero;

                foreach (var courseEntry in course.Entries)
                    if (watchedIdSet.Contains(courseEntry.Id))
                        watchedTimeSpan += courseEntry.Duration;
                    else
                        timeSpanLeftToWatch += courseEntry.Duration;

                var watchedDuration = FormatDuration(watchedTimeSpan);
                var durationLeft = FormatDuration(timeSpanLeftToWatch);
                return new WatchStatsResponse(watchedDuration, durationLeft, watchedIdList);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}