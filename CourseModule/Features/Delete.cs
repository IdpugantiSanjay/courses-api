using Contracts;
using CourseModule.Entities;
using Injectio.Attributes;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace CourseModule.Features;

public record DeleteCourseRequest(int ParentId, int Id) : IDeleteRequest<int, int>;

[RegisterScoped<IDelete<int, int, Course>>]
public sealed partial class CourseService : IDelete<int, int, Course>
{
    public async Task<OneOf<Success, NotFound>> Delete(IDeleteRequest<int, int> request, CancellationToken cancellationToken)
    {
        var deleted = await _context.Courses.Where(c => c.Id == request.Id).ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);
        if (deleted == 0) return new NotFound();
        return new Success();
    }
}