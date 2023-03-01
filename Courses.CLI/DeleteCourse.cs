using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

public class DeleteCourse
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DeleteCourse> _logger;

    public DeleteCourse(ILogger<DeleteCourse> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task Delete(int id)
    {
        var correlationId = Guid.NewGuid().ToString();
        using var _ = _logger.BeginScope("DELETE course: {Id}. with {CorrelationId}", id, correlationId);

        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"{id}");
        httpRequest.Headers.Add("x-correlation-id", correlationId);

        var deleteAsync = _httpClient.SendAsync(httpRequest);
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