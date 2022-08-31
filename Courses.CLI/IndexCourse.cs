using System.CommandLine;
using System.Net.Http.Json;
using Courses.Shared;
using Elastic.Apm;
using Elastic.Apm.Api;
using FFMpegCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

    public async Task Ingest()
    {
        var rootCommand = new RootCommand("CLI to Ingest Courses");

        var path = new Argument<DirectoryInfo>(
            "path",
            "course path");

        var author = new Option<string>("--author", () => string.Empty);
        var categories = new Option<string[]>("--categories", Array.Empty<string>);
        var platform = new Option<string>("--platform", () => string.Empty);

        rootCommand.AddArgument(path);
        rootCommand.AddOption(author);
        rootCommand.AddOption(categories);
        rootCommand.AddOption(platform);

        rootCommand.SetHandler(InternalIngestCourse, path, author, platform, categories);

        rootCommand.AddValidator(r =>
        {
            if (!r.GetValueForArgument(path).Exists) r.ErrorMessage = "The path provided is invalid.";
        });

        var args = Environment.GetCommandLineArgs()[1..];
        await rootCommand.InvokeAsync(args);
    }

    private async Task InternalIngestCourse(DirectoryInfo path, string author, string platform, string[] categories)
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
            _logger.LogCritical($"Invalid BACKEND_API Environment Variable value: {api}");
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
            _logger.LogError($"Couldn't connect to backend api: {backendUri}");
            return;
        }


        var walker = new CourseDirectoryWalker(path);
        var totalDuration = TimeSpan.Zero;
        var entries = new List<CreateCourseRequestEntry>();


        foreach (var (entry, index) in walker.Select((e, index) => (e, index)))
        {
            var tracerCurrentTransaction = Agent.Tracer.CurrentTransaction;
            var span = tracerCurrentTransaction.StartSpan("Calculate Video File Duration", ApiConstants.TypeExternal,
                "",
                ApiConstants.ActionQuery);

            span.SetLabel("FilePath", entry.FullName);
            var entryDuration = (await FFProbe.AnalyseAsync(entry.FullName)).Duration;
            span.End();

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


        var createCourseRequest =
            new CreateCourseRequest(path.Name, totalDuration, categories, false, author, platform, path.FullName,
                host, entries.ToArray());


        var httpResponseMessage = await http.PostAsJsonAsync("api/Courses", createCourseRequest);

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error indexing course, Response: {0}",
                await httpResponseMessage.Content.ReadFromJsonAsync<object>());

        await httpResponseMessage.Content.ReadAsStringAsync();
        httpResponseMessage.EnsureSuccessStatusCode();

        async Task<bool> IsBackendAvailable()
        {
            var healthResponse = await http.GetAsync("/healthz");
            return healthResponse.IsSuccessStatusCode;
        }
    }
}