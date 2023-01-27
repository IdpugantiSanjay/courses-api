using System.Diagnostics;
using System.Net.Http.Json;
using Courses.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

internal class ListCourses
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ListCourses> _logger;

    public ListCourses(IConfiguration configuration, ILogger<ListCourses> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task List()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var api = _configuration.GetValue<string>("BackendApi");
        var backendUri = new Uri(api);

        var http = new HttpClient(handler) { BaseAddress = backendUri };
        var getAsyncTask = http.GetAsync("api/Courses");
        HttpResponseMessage? httpResponseMessage = null;

        await AnsiConsole.Status()
            .StartAsync("Loading...", async _ => { httpResponseMessage = await getAsyncTask; });

        Debug.Assert(httpResponseMessage != null, nameof(httpResponseMessage) + " != null");

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error indexing course, Response: {ErrorResponse}",
                await httpResponseMessage.Content.ReadAsStringAsync());

        var response = await httpResponseMessage.Content.ReadFromJsonAsync<GetCoursesResponse>();

        Debug.Assert(response != null, nameof(response) + " != null");

        foreach (var (index, course) in response.Courses.Select((c, i) => (i, c)))
        {
            Console.Write($"{index + 1}. ");
            AnsiConsole.Console.WriteLine(course.Name, new Style(Color.Turquoise2));
        }
    }
}