using System.Net;
using System.Net.Http.Json;
using Bogus;
using CourseModule.Contracts;
using Courses.API;
using FluentAssertions;

namespace Watch.IntegrationTests;

public class WatchApiTests : IClassFixture<IntegrationTestFactory<Program>>
{
    private const string BaseUrl = "/api/v1/watch";
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory<Program> _factory;

    public WatchApiTests(IntegrationTestFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateDefaultClient();
    }

    private int CreateCourse()
    {
        var generator = new Faker<CreateRequestBody.Default>()
                .RuleFor(x => x.Duration, f => f.Date.Timespan())
                .RuleFor(x => x.Name, f => f.Company.CompanyName())
                .RuleFor(x => x.Entries, f => new List<CreateRequestBody.Default.Entry>
                {
                    new()
                    {
                        SequenceNumber = 1,
                        Name = f.Company.CompanyName(),
                        Duration = f.Date.Timespan()
                    }
                }.ToArray())
            ;
        var body = generator.Generate(1).First();
        var response = _client.PostAsJsonAsync("/api/v1/courses", body).GetAwaiter().GetResult();
        var createdId = response.Headers.Location!.ToString().Split("/").Last();
        return int.Parse(createdId);
    }

    [Fact]
    public async Task GetShouldReturnOk()
    {
        var courseId = CreateCourse();
        var response = await _client.GetAsync($"{BaseUrl}/{courseId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}