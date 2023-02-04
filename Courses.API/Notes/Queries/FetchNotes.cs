using Courses.API.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Notes.Queries;

public static class FetchNotes
{
    public record Request(int CourseId, int EntryId) : IRequest<Response>;

    public record Response(string? Note);

    private class Handler : IRequestHandler<Request, Response>
    {
        private readonly AppDbContext _context;

        public Handler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var note = await _context.Notes.Where(n => n.CourseId == request.CourseId && n.EntryId == request.EntryId).Select(x => x.Note)
                .FirstOrDefaultAsync(cancellationToken);
            return new Response(note);
        }
    }
}