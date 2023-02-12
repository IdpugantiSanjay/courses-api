using System.Diagnostics;
using System.Net.Http.Json;
using System.Xml;
using Courses.Shared;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Courses.CLI;

internal class Playlist
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Playlist> _logger;

    public Playlist(IConfiguration configuration, ILogger<Playlist> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Index(string playlistId, CancellationToken cancellationToken)
    {
        var api = _configuration.GetValue<string>("BackendApi");

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

        async Task<bool> IsBackendAvailable()
        {
            var healthResponse = await http.GetAsync("/healthz");
            return healthResponse.IsSuccessStatusCode;
        }

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

        var playlistRequest = service.Playlists.List("snippet");
        playlistRequest.Id = playlistId;

        var request = service.PlaylistItems.List("snippet");
        request.PlaylistId = playlistId;
        request.MaxResults = 50;

        var response = await request.ExecuteAsync(cancellationToken);
        var entries = new List<CreateCourseFromPlaylistRequestEntry>();
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

                entries.Add(new CreateCourseFromPlaylistRequestEntry(videoTitle, videoDurationInTimeSpan, ++sequenceNumber, video.Id));
            }

            request.PageToken = response.NextPageToken;
            response = await request.ExecuteAsync(cancellationToken);
        } while (!string.IsNullOrEmpty(response.NextPageToken));

        var playlistResponse = await playlistRequest.ExecuteAsync(cancellationToken);
        var playlist = playlistResponse.Items.First();
        var isCourseHd = decimal.Divide(hdVideosCount, entries.Count) * 100 > 50;
        var playlistDuration = entries.Aggregate(TimeSpan.Zero, (acc, curr) => curr.Duration + acc);
        var createCourseRequest = new CreateCourseFromPlaylistRequest(playlist.Snippet.Title, playlistDuration, Array.Empty<string>(), isCourseHd,
            playlist.Id, entries.ToArray());

        // foreach (var entry in createCourseRequest.Entries)
        // {
        //     Console.WriteLine($"{entry.Name} - {entry.Duration:g}");
        // }

        var postAsyncTask = http.PostAsJsonAsync("api/Courses/:playlist", createCourseRequest, cancellationToken);
        HttpResponseMessage? httpResponseMessage = null;

        await AnsiConsole.Status()
            .StartAsync("Inserting...", async _ => { httpResponseMessage = await postAsyncTask; });

        Debug.Assert(httpResponseMessage != null, nameof(httpResponseMessage) + " != null");

        if (!httpResponseMessage.IsSuccessStatusCode)
            _logger.LogError("Error indexing course, Response: {ErrorResponse}",
                await httpResponseMessage.Content.ReadAsStringAsync());

        httpResponseMessage.EnsureSuccessStatusCode();
        _logger.LogDebug("Added {CourseName} into Database", createCourseRequest.Name);
    }
}