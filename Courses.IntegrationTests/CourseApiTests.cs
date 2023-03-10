using System.Net;
using System.Net.Http.Json;
using CourseModule.Contracts;
using CourseModule.Database;
using Courses.API;
using FluentAssertions;
using Refit;

namespace Courses.IntegrationTests;

public class CourseApiTests : IClassFixture<IntegrationTestFactory<Program, CourseDbContext>>
{
    private const string BaseUrl = "/api/v1/courses";
    private readonly ICourseApi _api;
    private readonly HttpClient _client;

    public CourseApiTests(IntegrationTestFactory<Program, CourseDbContext> factory)
    {
        _client = factory.CreateDefaultClient();
        _api = RestService.For<ICourseApi>(_client);
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

        var courseId = await _api.Create(requestBody);
        courseId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task EmptyListShouldReturnOk()
    {
        var sut = async () => await _api.List();
        await sut.Should().NotThrowAsync();
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

        var sut = async () => await _api.Create(requestBody);
        await sut.Should().ThrowAsync<ApiException>();
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

        var createdId = await _api.Create(requestBody);
        var sut = async () => await _api.Delete(createdId);
        await sut.Should().NotThrowAsync<ApiException>();
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

        var createdId = await _api.Create(requestBody);
        var course = await _api.Get(createdId);
        course.Id.Should().BeGreaterThan(0);
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

        var createdId = await _api.Create(requestBody);
        createdId.Should().BeGreaterThan(0);
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

        var createdCourseId = await _api.Create(requestBody);
        var result = await _api.GetWithEntries(createdCourseId);
        result.Entries.Length.Should().Be(1);
        result.Entries.First().Name.Should().Be("Introduction");
        result.Entries.First().Duration.Should().Be(TimeSpan.FromMinutes(1));
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

        var createResponse = await _api.Create(requestBody);
        createResponse.Should().BeGreaterThan(0);

        var listResponse = await _api.ListWithEntries();
        listResponse.Items.Length.Should().BeGreaterThan(0);
    }
}