using CourseModule.Contracts;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

public class DeleteCourse
{
    private readonly ICourseApi _api;
    private readonly ILogger<DeleteCourse> _logger;

    public DeleteCourse(ILogger<DeleteCourse> logger, ICourseApi api)
    {
        _logger = logger;
        _api = api;
    }

    public async Task Delete(int id)
    {
        var correlationId = Guid.NewGuid().ToString();
        using var _ = _logger.BeginScope("DELETE course: {Id}. with {CorrelationId}", id, correlationId);
        var deleteAsync = _api.Delete(id, correlationId);
        await AnsiConsole.Status()
            .StartAsync("Deleting...", async _ => { await deleteAsync; });
        AnsiConsole.Console.WriteLine("Course Deleted Successfully", new Style(Color.DeepPink3));
    }
}