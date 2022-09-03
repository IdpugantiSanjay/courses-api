using System.Diagnostics;
using System.Net.Http.Json;
using Courses.Shared;
using FFMpegCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console;
// using Elastic.Apm;
// using Elastic.Apm.Api;

namespace Courses.CLI;

internal class IndexCourse
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexCourse> _logger;

    public IndexCourse(IConfiguration configuration, ILogger<IndexCourse> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Ingest(DirectoryInfo path, string author, string platform, string[] categories)
    {
        var api = _configuration.GetValue<string>("BackendApi");
        var host = _configuration.GetValue<string>("HostMachine");


        if (string.IsNullOrWhiteSpace(host))
        {
            _logger.LogCritical("Host name not provided");
            return;
        }

        if (string.IsNullOrWhiteSpace(api) || !Uri.IsWellFormedUriString(api, UriKind.Absolute))
        {
            _logger.LogCritical("Invalid BACKEND_API Environment Variable value: {Api}", api);
            return;
        }

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };


        var backendUri = new Uri(api);

        var http = new HttpClient(handler) { BaseAddress = backendUri };

        if (!await IsBackendAvailable())
        {
            _logger.LogError("Couldn't connect to backend api: {BackendUri}", backendUri);
            return;
        }


        var walker = new CourseDirectoryWalker(path);
        var totalDuration = TimeSpan.Zero;
        var entries = new List<CreateCourseRequestEntry>();

        await AnsiConsole.Status().StartAsync("Processing...", async _ =>
        {
            foreach (var (entry, index) in walker.Select((e, index) => (e, index)))
            {
                // var tracerCurrentTransaction = Agent.Tracer.CurrentTransaction;
                // var span = tracerCurrentTransaction.StartSpan("Calculate Video File Duration",
                //     ApiConstants.TypeExternal,
                //     "",
                //     ApiConstants.ActionQuery);

                // span.SetLabel("FilePath", entry.FullName);
                var entryDuration = (await FFProbe.AnalyseAsync(entry.FullName)).Duration;
                // span.End();

                totalDuration += entryDuration;
                var section = entry switch
                {
                    FileInfo { Directory: not null } fileInfo when fileInfo.Directory.Name != path.Name => fileInfo
                        .Directory
                        .Name,
                    _ => string.Empty
                };
                entries.Add(new CreateCourseRequestEntry(entry.Name, entryDuration, index + 1, section));
            }
        });

        var createCourseRequest =
            new CreateCourseRequest(path.Name, totalDuration, categories, false, author, platform, path.FullName,
                host, entries.ToArray());

        var postAsyncTask = http.PostAsJsonAsync("api/Courses", createCourseRequest);
        HttpResponseMessage? httpResponseMessage = null;

        await AnsiConsole.Status()
            .StartAsync("Inserting...", async _ => { httpResponseMessage = await postAsyncTask; });


        Debug.Assert(httpResponseMessage != null, nameof(httpResponseMessage) + " != null");

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error indexing course, Response: {ErrorResponse}",
                await httpResponseMessage.Content.ReadAsStringAsync());

        httpResponseMessage.EnsureSuccessStatusCode();
        _logger.LogInformation("Inserted {CourseName} into Database", createCourseRequest.Name);

        async Task<bool> IsBackendAvailable()
        {
            var healthResponse = await http.GetAsync("/healthz");
            return healthResponse.IsSuccessStatusCode;
        }
    }
}