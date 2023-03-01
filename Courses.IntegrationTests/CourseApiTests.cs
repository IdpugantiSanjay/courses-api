using System.Net;
using System.Net.Http.Json;
using Contracts;
using CourseModule.Contracts;
using CourseModule.Database;
using Courses.API;
using FluentAssertions;

namespace Courses.IntegrationTests;

public class CourseApiTests : IClassFixture<IntegrationTestFactory<Program, CourseDbContext>>
{
    private const string BaseUrl = "/api/v1/courses";
    private readonly HttpClient _client;

    public CourseApiTests(IntegrationTestFactory<Program, CourseDbContext> factory)
    {
        _client = factory.CreateDefaultClient();
    }

    [Fact]
    public async Task PostShouldCreate()
    {
        var requestBody = new CreateRequestBody.Default
        {
            Categories = Array.Empty<string>(),
            Duration = TimeSpan.FromMinutes(1),
            Entries = new[]
            {
                new CreateRequestBody.Default.Entry
                {
                    SequenceNumber = 1,
                    Name = "Introduction",
                    Duration = TimeSpan.FromMinutes(1)
                }
            },
            Name = "Test Course",
            IsHighDefinition = false
        };

        var response = await _client.PostAsJsonAsync(BaseUrl, requestBody);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task ListShouldReturnOk()
    {
        var response = await _client.GetAsync(BaseUrl);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostShouldReturnBadRequest()
    {
        var requestBody = new CreateRequestBody.Default
        {
            Categories = Array.Empty<string>(),
            Duration = TimeSpan.Zero,
            Entries = Array.Empty<CreateRequestBody.Default.Entry>(),
            Name = string.Empty,
            IsHighDefinition = false
        };

        var response = await _client.PostAsJsonAsync(BaseUrl, requestBody);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteShouldReturnNoContent()
    {
        var requestBody = new CreateRequestBody.Default
        {
            Categories = Array.Empty<string>(),
            Duration = TimeSpan.FromMinutes(1),
            Entries = new[]
            {
                new CreateRequestBody.Default.Entry
                {
                    SequenceNumber = 1,
                    Name = "Introduction",
                    Duration = TimeSpan.FromMinutes(1)
                }
            },
            Name = "Test Course I",
            IsHighDefinition = false
        };

        var response = await _client.PostAsJsonAsync(BaseUrl, requestBody);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdId = response.Headers.Location!.ToString().Split("/").Last();

        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/{createdId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetShouldReturnOk()
    {
        var requestBody = new CreateRequestBody.Default
        {
            Categories = Array.Empty<string>(),
            Duration = TimeSpan.FromMinutes(1),
            Entries = new[]
            {
                new CreateRequestBody.Default.Entry
                {
                    SequenceNumber = 1,
                    Name = "Introduction",
                    Duration = TimeSpan.FromMinutes(1)
                }
            },
            Name = "Test Course II",
            IsHighDefinition = false
        };

        var response = await _client.PostAsJsonAsync(BaseUrl, requestBody);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdId = response.Headers.Location!.ToString().Split("/").Last();

        var getResponse = await _client.GetAsync($"{BaseUrl}/{createdId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostShouldCreatePlaylist()
    {
        var requestBody = new CreateRequestBody.Playlist
        {
            Categories = Array.Empty<string>(),
            Duration = TimeSpan.FromMinutes(1),
            Entries = new[]
            {
                new CreateRequestBody.Playlist.Entry
                {
                    SequenceNumber = 1,
                    Name = "Introduction",
                    VideoId = "1",
                    Duration = TimeSpan.FromMinutes(1)
                }
            },
            Name = "Test Course IV",
            IsHighDefinition = false,
            PlaylistId = "1"
        };

        var response = await _client.PostAsJsonAsync(BaseUrl, requestBody);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DeleteShouldReturnNotFound()
    {
        var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/0");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"{BaseUrl}/0");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PostShouldReturnBadRequestOnInvalidRequestBody()
    {
        var body = new
        {
            Property1 = "Value"
        };
        var response = await _client.PostAsJsonAsync(BaseUrl, body);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetEntriesShouldReturnOk()
    {
        var name = Guid.NewGuid().ToString();
        var requestBody = new CreateRequestBody.Default
        {
            Categories = Array.Empty<string>(),
            Duration = TimeSpan.FromMinutes(1),
            Entries = new[]
            {
                new CreateRequestBody.Default.Entry
                {
                    SequenceNumber = 1,
                    Name = "Introduction",
                    Duration = TimeSpan.FromMinutes(1)
                }
            },
            Name = name,
            IsHighDefinition = false
        };

        var postResponse = await _client.PostAsJsonAsync(BaseUrl, requestBody);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdId = postResponse.Headers.Location!.ToString().Split("/").Last();

        var response = await _client.GetAsync($"{BaseUrl}/{createdId}?view=Entries");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<CourseResponse.WithEntries>();
        result!.Entries.Length.Should().Be(1);
        result.Entries.First().Name.Should().Be("Introduction");
        result.Name.Should().Be(name);
    }

    [Fact]
    public async Task ListWithEntriesShouldReturnOk()
    {
        var name = Guid.NewGuid().ToString();
        var requestBody = new CreateRequestBody.Default
        {
            Categories = Array.Empty<string>(),
            Duration = TimeSpan.FromMinutes(1),
            Entries = new[]
            {
                new CreateRequestBody.Default.Entry
                {
                    SequenceNumber = 1,
                    Name = "Introduction",
                    Duration = TimeSpan.FromMinutes(1)
                }
            },
            Name = name,
            IsHighDefinition = false
        };

        var postResponse = await _client.PostAsJsonAsync(BaseUrl, requestBody);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await _client.GetAsync($"{BaseUrl}?view=Entries");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ListResponse<CourseResponse.WithEntries>>();
        result!.Items.Length.Should().BeGreaterThan(0);
    }
}