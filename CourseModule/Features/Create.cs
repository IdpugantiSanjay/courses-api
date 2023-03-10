using Contracts;
using CourseModule.Contracts;
using CourseModule.Entities;
using Injectio.Attributes;
using Mapster;

namespace CourseModule.Features;

public record CreateCourseRequest : ICreateRequest<int, CreateRequestBody>
{
    public required int ParentId { get; init; }
    public required CreateRequestBody Body { get; init; }
}

[RegisterScoped<ICreate<int, CreateRequestBody, int>>]
public sealed partial class CourseService : ICreate<int, CreateRequestBody, int>
{
    public async Task<int> Create(ICreateRequest<int, CreateRequestBody> request, CancellationToken cancellationToken)
    {
        try
        {
            var course = request.Body.Adapt<Course>();
            _context.Courses.Add(course);
            await _context.SaveChangesAsync(cancellationToken);
            return course.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}