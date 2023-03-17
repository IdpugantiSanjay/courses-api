using System.Diagnostics;
using CourseModule.Contracts;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

public class GetCourse
{
    private readonly ICourseApi _api;
    private readonly ILogger<GetCourse> _logger;

    public GetCourse(ILogger<GetCourse> logger, ICourseApi api)
    {
        _logger = logger;
        _api = api;
    }

    public async Task Get(int id)
    {
        var correlationId = Guid.NewGuid().ToString();
        using var _ = _logger.BeginScope("GET course: {Id}. with {CorrelationId}", id, correlationId);

        var getAsyncTask = _api.GetWithEntries(id, correlationId);
        CourseResponse.WithEntries? response = null;

        await AnsiConsole.Status()
            .StartAsync("Loading...", async _ => { response = await getAsyncTask; });

        Debug.Assert(response != null, nameof(response) + " != null");

        foreach (var entry in response.Entries)
        {
            Console.Write($"{entry.Id}. ");
            AnsiConsole.Console.WriteLine(entry.Name, new Style(Color.Turquoise2));
        }
    }
}