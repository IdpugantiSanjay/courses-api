using System.Diagnostics;
using Contracts;
using CourseModule.Contracts;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

internal class ListCourses
{
    private readonly ICourseApi _api;
    private readonly ILogger<ListCourses> _logger;

    public ListCourses(ILogger<ListCourses> logger, ICourseApi api)
    {
        _logger = logger;
        _api = api;
    }

    public async Task List()
    {
        var correlationId = Guid.NewGuid().ToString();
        using var _ = _logger.BeginScope("Starting listing courses. {CorrelationId}", correlationId);

        var listTask = _api.List(CourseView.Default, correlationId);
        ListResponse<CourseResponse>? response = null;

        await AnsiConsole.Status()
            .StartAsync("Loading...", async _ => { response = await listTask; });

        Debug.Assert(response != null, nameof(response) + " != null");
        _logger.LogInformation("GET request returned with success code");

        if (response.Items.Length == 0) AnsiConsole.Console.WriteLine("No courses found.", new Style(Color.Yellow));

        foreach (var course in response.Items)
        {
            Console.Write($"{course.Id}. ");
            AnsiConsole.Console.WriteLine(course.Name, new Style(Color.Turquoise2));
        }
    }
}