using Courses.API.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Courses.Commands;

public class Update
{
    public record Request(int CourseId, List<int> TagIdList) : IRequest<Unit>;

    private class Handler : IRequestHandler<Request, Unit>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var course = await _context.Courses.Include(c => c.Tags).FirstAsync(c => c.Id == request.CourseId, cancellationToken);
            course.Tags.RemoveAll(_ => true);
            var tags = await _context.Tags.Where(t => request.TagIdList.Contains(t.Id)).ToListAsync(cancellationToken);
            course.Tags = tags;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}