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

    private static async Task<(TimeSpan, bool)> VideoProperties(string filePath)
    {
        var mediaAnalysis = await FFProbe.AnalyseAsync(filePath);
        var videoStream = mediaAnalysis.VideoStreams.First();
        return (mediaAnalysis.Duration, videoStream.Width >= 1920);
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


        var walker = new CourseDirectoryWalker(path);
        var totalDuration = TimeSpan.Zero;
        var entries = new List<CreateCourseRequestEntry>();
        var hdVideosCount = 0;

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
                var (entryDuration, isHd) = await VideoProperties(entry.FullName);
                if (isHd) hdVideosCount++;

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

        var entriesArray = entries.ToArray();

        if (entriesArray.Length == 0) return;


        var isCourseHd = decimal.Divide(hdVideosCount, entriesArray.Length) * 100 > 50;

        if (!path.Name.Contains($"{totalDuration:G}"))
        {
            var moveToPath =
                $"{path.Parent?.FullName}/{path.Name} {(isCourseHd ? "[HD]" : "")} {string.Concat(totalDuration.ToString("G").TakeWhile(c => c != '.'))}";
            path.MoveTo(moveToPath);
        }

        var createCourseRequest =
            new CreateCourseRequest(path.Name, totalDuration, categories, isCourseHd, author, platform, path.FullName,
                host, entriesArray);

        if (!await IsBackendAvailable())
        {
            _logger.LogError("Couldn't connect to backend api: {BackendUri}", backendUri);
            return;
        }

        var postAsyncTask = http.PostAsJsonAsync("api/Courses", createCourseRequest);
        HttpResponseMessage? httpResponseMessage = null;

        await AnsiConsole.Status()
            .StartAsync("Inserting...", async _ => { httpResponseMessage = await postAsyncTask; });

        Debug.Assert(httpResponseMessage != null, nameof(httpResponseMessage) + " != null");

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error indexing course, Response: {ErrorResponse}",
                await httpResponseMessage.Content.ReadAsStringAsync());

        httpResponseMessage.EnsureSuccessStatusCode();
        _logger.LogDebug("Added {CourseName} into Database", createCourseRequest.Name);

        async Task<bool> IsBackendAvailable()
        {
            var healthResponse = await http.GetAsync("/healthz");
            return healthResponse.IsSuccessStatusCode;
        }
    }
}