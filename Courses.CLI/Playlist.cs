using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Xml;
using CourseModule.Contracts;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

internal class HttpInterceptor : IHttpExecuteInterceptor
{
    private readonly ILogger<HttpInterceptor> _logger;

    public HttpInterceptor(ILogger<HttpInterceptor> logger)
    {
        _logger = logger;
    }

    public Task InterceptAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending HTTP request for {Route} {Method} {Headers}", request.RequestUri!.ToString(), request.Method, request.Headers);
        return Task.CompletedTask;
        ;
    }
}

internal class Playlist
{
    private readonly HttpClient _httpClient;
    private readonly HttpInterceptor _interceptor;

    private readonly ILogger<Playlist> _logger;

    public Playlist(ILogger<Playlist> logger, HttpClient httpClient, HttpInterceptor interceptor)
    {
        _logger = logger;
        _httpClient = httpClient;
        _interceptor = interceptor;
    }

    public async Task Index(string playlistId, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString();
        using var _ = _logger.BeginScope("Process Youtube playlistId: {PlaylistId}. {CorrelationId}", playlistId, correlationId);

        string[] scopes = { YouTubeService.ScopeConstants.Youtube };
        const string applicationName = "courses";
        var credentialsPath = Environment.GetEnvironmentVariable("YOUTUBE_DATA_API_CREDENTIALS_PATH");
        var credentialsStore = Environment.GetEnvironmentVariable("YOUTUBE_DATA_API_CREDENTIALS_STORE_PATH");

        if (string.IsNullOrEmpty(credentialsPath))
        {
            _logger.LogError("YOUTUBE_DATA_API_CREDENTIALS_PATH environment variable not provided");
            return;
        }

        if (string.IsNullOrEmpty(credentialsStore))
        {
            _logger.LogError("YOUTUBE_DATA_API_CREDENTIALS_STORE_PATH environment variable not provided");
            return;
        }

        await using var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            (await GoogleClientSecrets.FromStreamAsync(stream, cancellationToken)).Secrets,
            scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credentialsStore, true));

        var service = new YouTubeService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = applicationName
        });

        service.HttpClient.MessageHandler.AddExecuteInterceptor(_interceptor);

        var request = service.PlaylistItems.List("snippet");
        request.PlaylistId = playlistId;
        request.MaxResults = 50;

        var stopWatch = Stopwatch.StartNew();
        _logger.LogInformation("Started processing playlist: {PlaylistId}", playlistId);
        var response = await request.ExecuteAsync(cancellationToken);
        var entries = new List<CreateRequestBody.Playlist.Entry>();
        var sequenceNumber = 0;
        var hdVideosCount = 0;

        do
        {
            foreach (var responseItem in response.Items)
            {
                var videoDetailsRequest = service.Videos.List("contentDetails");
                videoDetailsRequest.Id = responseItem.Snippet.ResourceId.VideoId;
                var videoDetailsResponse = await videoDetailsRequest.ExecuteAsync(cancellationToken);
                var video = videoDetailsResponse.Items.First();
                var contentDetailsDuration =
                    video!.ContentDetails.Duration;
                var videoDurationInTimeSpan = XmlConvert.ToTimeSpan(contentDetailsDuration);
                var videoTitle = responseItem.Snippet.Title.Replace("/", "-");

                if (video.ContentDetails.Definition == "hd") hdVideosCount++;

                // new CreateCourseFromPlaylistRequestEntry(videoTitle, videoDurationInTimeSpan, ++sequenceNumber, video.Id)
                var playlistEntry = new CreateRequestBody.Playlist.Entry
                    { Name = videoTitle, VideoId = video.Id, Duration = videoDurationInTimeSpan, SequenceNumber = ++sequenceNumber };
                entries.Add(playlistEntry);
            }

            request.PageToken = response.NextPageToken;
            response = await request.ExecuteAsync(cancellationToken);
        } while (!string.IsNullOrEmpty(response.NextPageToken));

        var playlistRequest = service.Playlists.List("snippet");
        playlistRequest.Id = playlistId;
        var playlistResponse = await playlistRequest.ExecuteAsync(cancellationToken);

        stopWatch.Stop();
        _logger.LogInformation("Done processing playlist: {PlaylistId}. Tool {@ElapsedTime}", playlistId, stopWatch.Elapsed);

        var playlist = playlistResponse.Items.First();
        var isCourseHd = decimal.Divide(hdVideosCount, entries.Count) * 100 > 50;
        var playlistDuration = entries.Aggregate(TimeSpan.Zero, (acc, curr) => curr.Duration + acc);

        var body = new CreateRequestBody.Playlist
        {
            Name = playlist.Snippet.Title,
            Duration = playlistDuration,
            PlaylistId = playlist.Id,
            Entries = entries.ToArray(),
            IsHighDefinition = isCourseHd
        };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "")
        {
            Content = content
        };
        httpRequest.Headers.Add("x-correlation-id", correlationId);

        _logger.LogInformation("Sending Playlist contents to API: {@Body}", body);
        var postAsyncTask = _httpClient.SendAsync(httpRequest, cancellationToken);
        HttpResponseMessage? httpResponseMessage = null;

        await AnsiConsole.Status()
            .StartAsync("Inserting...", async _ => { httpResponseMessage = await postAsyncTask; });

        Debug.Assert(httpResponseMessage != null, nameof(httpResponseMessage) + " != null");

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error indexing course, Response: {ErrorResponse}",
                await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));

        httpResponseMessage.EnsureSuccessStatusCode();
        _logger.LogDebug("Added {CourseName} into Database", body.Name);
    }
}