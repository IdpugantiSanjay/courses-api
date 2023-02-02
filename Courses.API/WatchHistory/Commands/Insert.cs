using Courses.API.Database;
using Courses.Shared;
using MediatR;

namespace Courses.API.WatchHistory.Commands;

public class Insert : IRequestHandler<SetWatchedRequest, Unit>
{
    private readonly AppDbContext _context;

    public Insert(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(SetWatchedRequest request, CancellationToken cancellationToken)
    {
        _context.Watched.Add(new WatchHistory { CourseId = request.CourseId, EntryId = request.CourseEntryId, CreatedAt = DateTimeOffset.UtcNow });
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}