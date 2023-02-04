using Courses.API.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Notes.Queries;

public static class FetchEntriesWithNotes
{
    public record Request(int CourseId) : IRequest<Response>;

    private record Response(List<int> EntryIdList);

    private class Handler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var record = await _context.Notes.Where(n => n.CourseId == request.CourseId).Select(x => x.EntryId).ToListAsync(cancellationToken);
            return new Response(record);
        }
    }
}