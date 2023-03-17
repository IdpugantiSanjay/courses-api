using System.Diagnostics;
using CourseModule.Contracts;
using FFMpegCore;
using Microsoft.Extensions.Logging;
using Spectre.Console;

// using Elastic.Apm;
// using Elastic.Apm.Api;

namespace Courses.CLI;

internal class IndexCourse
{
    private readonly ICourseApi _api;
    private readonly ILogger<IndexCourse> _logger;

    public IndexCourse(ILogger<IndexCourse> logger, ICourseApi api)
    {
        _logger = logger;
        _api = api;
    }

    private static async Task<(TimeSpan, bool)> GetVideoProperties(string filePath)
    {
        var mediaAnalysis = await FFProbe.AnalyseAsync(filePath);
        var videoStream = mediaAnalysis.VideoStreams.First();
        return (mediaAnalysis.Duration, videoStream.Width >= 1920);
    }

    public async Task Ingest(DirectoryInfo path, string[] categories)
    {
        var correlationId = Guid.NewGuid().ToString();
        var stopWatch = new Stopwatch();
        using var _ = _logger.BeginScope("Processing {Path}. With {CorrelationId}", path.ToString(), correlationId);

        var walker = new CourseDirectoryWalker(path);
        var totalDuration = TimeSpan.Zero;
        var entries = new List<CreateRequestBody.Default.Entry>();
        var hdVideosCount = 0;

        await AnsiConsole.Status().StartAsync("Processing...", async _ =>
        {
            stopWatch.Start();
            foreach (var (entry, index) in walker.Select((e, index) => (e, index)))
            {
                var (entryDuration, isHd) = await GetVideoProperties(entry.FullName);
                if (isHd) hdVideosCount++;

                totalDuration += entryDuration;
                var section = entry switch
                {
                    FileInfo { Directory: not null } fileInfo when fileInfo.Directory.Name != path.Name => fileInfo
                        .Directory
                        .Name,
                    _ => string.Empty
                };

                var defaultEntry = new CreateRequestBody.Default.Entry
                    { Name = entry.Name, Duration = entryDuration, SequenceNumber = index + 1, Section = section };

                entries.Add(defaultEntry);
            }

            stopWatch.Stop();
            _logger.LogInformation("Done processing {Path}. Took {@ElapsedTime}", path.ToString(), stopWatch.Elapsed);
        });

        var entriesArray = entries.ToArray();

        if (entriesArray.Length == 0) return;

        var isCourseHd = decimal.Divide(hdVideosCount, entriesArray.Length) * 100 > 50;

        if (!path.Name.Contains(FormatTimeSpan(totalDuration)))
        {
            var moveToPath =
                $"{path.Parent?.FullName}/{path.Name}{(isCourseHd ? " [HD] " : " ")}[{FormatTimeSpan(totalDuration)}]";
            path.MoveTo(moveToPath);
        }

        var body = new CreateRequestBody.Default
            { Name = path.Name, Duration = totalDuration, Categories = categories, IsHighDefinition = isCourseHd, Entries = entries.ToArray() };

        var createTask = _api.Create(body, correlationId);
        var createdId = 0;

        _logger.LogInformation("Making API request to insert {CourseName}", body.Name);

        await AnsiConsole.Status()
            .StartAsync("Inserting...", async _ => { createdId = await createTask; });
        Debug.Assert(createdId != default, nameof(createdId) + " != 0");
        _logger.LogInformation("Added {CourseName} into Database", body.Name);
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        var duration = timeSpan.Days > 0
            ? $"{timeSpan.Days}d{timeSpan.Hours:0}h{timeSpan.Minutes:00}m"
            : timeSpan.Hours > 0
                ? $"{timeSpan.Hours:0}h{timeSpan.Minutes:0}m"
                : $"{timeSpan.Minutes:0}m";
        return duration;
    }
}