using System.Diagnostics;
using System.Net.Http.Json;
using Courses.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

public class GetCourse
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetCourse> _logger;

    public GetCourse(IConfiguration configuration, ILogger<GetCourse> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Get(int id)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var api = _configuration.GetValue<string>("BackendApi");
        var backendUri = new Uri(api);

        var http = new HttpClient(handler) { BaseAddress = backendUri };
        var getAsyncTask = http.GetAsync($"api/Courses/{id}");
        HttpResponseMessage? httpResponseMessage = null;

        await AnsiConsole.Status()
            .StartAsync("Loading...", async _ =>
            {
                // await Task.Delay(2_000);
                httpResponseMessage = await getAsyncTask;
            });

        Debug.Assert(httpResponseMessage != null, nameof(httpResponseMessage) + " != null");

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error indexing course, Response: {ErrorResponse}",
                await httpResponseMessage.Content.ReadAsStringAsync());

        var response = await httpResponseMessage.Content.ReadFromJsonAsync<GetByIdCourseView>();

        Debug.Assert(response != null, nameof(response) + " != null");

        foreach (var entry in response.Entries)
        {
            Console.Write($"{entry.Id}. ");
            AnsiConsole.Console.WriteLine(entry.Name, new Style(Color.Turquoise2));
        }
    }
}