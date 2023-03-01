using System.Diagnostics;
using System.Net.Http.Json;
using CourseModule.Contracts;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

public class GetCourse
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GetCourse> _logger;

    public GetCourse(ILogger<GetCourse> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task Get(int id)
    {
        var correlationId = Guid.NewGuid().ToString();
        using var _ = _logger.BeginScope("GET course: {Id}. with {CorrelationId}", id, correlationId);

        var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{id}?view={nameof(CourseView.Entries)}");
        httpRequest.Headers.Add("x-correlation-id", correlationId);

        var getAsyncTask = _httpClient.SendAsync(httpRequest);
        HttpResponseMessage? httpResponseMessage = null;

        await AnsiConsole.Status()
            .StartAsync("Loading...", async _ => { httpResponseMessage = await getAsyncTask; });

        Debug.Assert(httpResponseMessage != null, nameof(httpResponseMessage) + " != null");

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error fetching course, Response: {ErrorResponse}",
                await httpResponseMessage.Content.ReadAsStringAsync());

        var response = await httpResponseMessage.Content.ReadFromJsonAsync<CourseResponse.WithEntries>();

        Debug.Assert(response != null, nameof(response) + " != null");

        foreach (var entry in response.Entries)
        {
            Console.Write($"{entry.Id}. ");
            AnsiConsole.Console.WriteLine(entry.Name, new Style(Color.Turquoise2));
        }
    }
}