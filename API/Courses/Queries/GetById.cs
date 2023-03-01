// using Courses.API.Database;
// using Courses.Shared;
// using JetBrains.Annotations;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
// using static Courses.API.Courses.Functions;
//
// namespace Courses.API.Courses.Queries;
//
// [UsedImplicitly]
// public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdRequest, GetByIdCourseView>
// {
//     private readonly AppDbContext _context;
//
//     public GetCourseByIdHandler(AppDbContext context)
//     {
//         _context = context;
//     }
//
//     public async Task<GetByIdCourseView> Handle(GetCourseByIdRequest request, CancellationToken cancellationToken)
//     {
//         var query = _context.Courses
//                 .Where(c => c.Id == request.Id)
//                 .Include(c => c.Entries)
//             ;
//
//         var result = await query.FirstAsync(cancellationToken);
//
//         var entries =
//             result.Entries.Select(e => new GetByIdCourseEntryView(e.Id, e.Name, FormatDuration(e.Duration), e.SequenceNumber, e.Section, false, e.VideoId));
//
//         return new GetByIdCourseView(result.Id, result.Name, FormatDuration(result.Duration), Array.Empty<string>(), result.IsHighDefinition, result.PlaylistId,
//             entries.ToArray());
//     }
// }

