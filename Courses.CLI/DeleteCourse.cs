using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

public class DeleteCourse
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeleteCourse> _logger;

    public DeleteCourse(IConfiguration configuration, ILogger<DeleteCourse> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Delete(int id)
    {
        var api = _configuration.GetValue<string>("BackendApi");
        var backendUri = new Uri(api);

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var http = new HttpClient(handler) { BaseAddress = backendUri };
        var deleteAsync = http.DeleteAsync($"api/Courses/{id}");
        HttpResponseMessage? httpResponseMessage = null;

        await AnsiConsole.Status()
            .StartAsync("Deleting...", async _ =>
            {
                // await Task.Delay(2_000);
                httpResponseMessage = await deleteAsync;
            });

        Debug.Assert(httpResponseMessage != null, nameof(httpResponseMessage) + " != null");

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error Deleting course, Response: {ErrorResponse}",
                await httpResponseMessage.Content.ReadAsStringAsync());
        else
            AnsiConsole.Console.WriteLine("Course Deleted Successfully", new Style(Color.DeepPink3));
    }
}