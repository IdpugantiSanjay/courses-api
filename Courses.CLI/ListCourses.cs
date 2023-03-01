using System.Diagnostics;
using System.Net.Http.Json;
using Contracts;
using CourseModule.Contracts;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

internal class ListCourses
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ListCourses> _logger;

    public ListCourses(ILogger<ListCourses> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task List()
    {
        var correlationId = Guid.NewGuid().ToString();
        using var _ = _logger.BeginScope("Starting listing courses. {CorrelationId}", correlationId);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "");
        httpRequestMessage.Headers.Add("x-correlation-id", correlationId);

        var getAsyncTask = _httpClient.SendAsync(httpRequestMessage);
        HttpResponseMessage? httpResponseMessage = null;

        await AnsiConsole.Status()
            .StartAsync("Loading...", async _ => { httpResponseMessage = await getAsyncTask; });

        Debug.Assert(httpResponseMessage != null, nameof(httpResponseMessage) + " != null");

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error indexing course, Response: {ErrorResponse}",
                await httpResponseMessage.Content.ReadAsStringAsync());

        httpResponseMessage.EnsureSuccessStatusCode();

        _logger.LogInformation("GET request returned with success code");

        var response = await httpResponseMessage.Content.ReadFromJsonAsync<ListResponse<CourseResponse.Default>>();

        Debug.Assert(response != null, nameof(response) + " != null");

        if (response.Items.Length == 0) AnsiConsole.Console.WriteLine("No courses found.", new Style(Color.Yellow));

        foreach (var course in response.Items)
        {
            Console.Write($"{course.Id}. ");
            AnsiConsole.Console.WriteLine(course.Name, new Style(Color.Turquoise2));
        }
    }
}