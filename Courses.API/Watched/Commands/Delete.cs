using Courses.API.Database;
using Courses.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Watched.Commands;

public class Delete : IRequestHandler<DeletedWatchedRequest, Unit>
{
    private readonly AppDbContext _context;

    public Delete(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeletedWatchedRequest request, CancellationToken cancellationToken)
    {
        await _context.Watched.Where(w => w.CourseId == request.CourseId && w.EntryId == request.CourseEntryId)
            .ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);

        return Unit.Value;
    }
}