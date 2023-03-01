// using Courses.API.Database;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
//
// namespace Courses.API.Notes.Commands;
//
// public static class Upsert
// {
//     public record UpsertRequest(int CourseId, int EntryId, string Note) : IRequest<Unit>;
//
//     public class UpsertHandler : IRequestHandler<UpsertRequest, Unit>
//     {
//         private readonly AppDbContext _context;
//
//         public UpsertHandler(AppDbContext context)
//         {
//             _context = context;
//         }
//
//         public async Task<Unit> Handle(UpsertRequest request, CancellationToken cancellationToken)
//         {
//             if (await _context.Notes.FirstOrDefaultAsync(n => n.CourseId == request.CourseId && n.EntryId == request.EntryId, cancellationToken) is not null)
//                 await _context.Notes.Where(n => n.CourseId == request.CourseId && n.EntryId == request.EntryId).ExecuteUpdateAsync
//                     (n => n.SetProperty(x => x.Note, request.Note), cancellationToken).ConfigureAwait(false);
//             else
//                 _context.Notes.Add(new Notes { Note = request.Note, CourseId = request.CourseId, EntryId = request.EntryId });
//             await _context.SaveChangesAsync(cancellationToken);
//             return Unit.Value;
//         }
//     }
// }

