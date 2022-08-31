using Courses.API.Database;
using Courses.Shared;
using JetBrains.Annotations;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Courses.API.Courses.Queries;

[UsedImplicitly]
public class GetCourseByIdHandler : IRequestHandler<GetCourseByIdRequest, GetByIdCourseView>
{
    private readonly AppDbContext _context;

    public GetCourseByIdHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetByIdCourseView> Handle(GetCourseByIdRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Courses
                .Where(c => c.Id == request.Id)
                .Include(c => c.Entries)
                .Include(c => c.Author)
                .Include(c => c.Platform)
            ;

        var result = await query.FirstAsync(cancellationToken);
        return result.Adapt<GetByIdCourseView>();
    }
}