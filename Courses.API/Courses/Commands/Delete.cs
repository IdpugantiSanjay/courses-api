using Courses.API.Database;
using Courses.Shared;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Courses.Commands;

[UsedImplicitly]
public class DeleteCourseHandler : IRequestHandler<DeleteCourseByIdRequest>
{
    private readonly AppDbContext _context;
    private readonly ILogger<DeleteCourseHandler> _logger;

    public DeleteCourseHandler(AppDbContext context, ILogger<DeleteCourseHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteCourseByIdRequest request, CancellationToken cancellationToken)
    {
        var courseToDelete = await _context.Courses.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        ArgumentNullException.ThrowIfNull(courseToDelete);
        _context.Courses.Remove(courseToDelete);
        return Unit.Value;
    }
}