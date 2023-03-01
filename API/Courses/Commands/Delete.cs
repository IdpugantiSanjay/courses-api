// using Courses.API.Database;
// using Courses.Shared;
// using JetBrains.Annotations;
// using MediatR;
//
// namespace Courses.API.Courses.Commands;
//
// [UsedImplicitly]
// public class DeleteCourseHandler : IRequestHandler<DeleteCourseByIdRequest>
// {
//     private readonly AppDbContext _context;
//     private readonly ILogger<DeleteCourseHandler> _logger;
//
//     public DeleteCourseHandler(AppDbContext context, ILogger<DeleteCourseHandler> logger)
//     {
//         _context = context;
//         _logger = logger;
//     }
//
//     public async Task<Unit> Handle(DeleteCourseByIdRequest request, CancellationToken cancellationToken)
//     {
//         var courseToDelete = new Course { Id = request.Id };
//         _context.Courses.Attach(courseToDelete);
//         _context.Courses.Remove(courseToDelete);
//         await _context.SaveChangesAsync(cancellationToken);
//         _logger.LogInformation("Deleted Course with Id: {Id}", request.Id);
//         return Unit.Value;
//     }
// }

