using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Courses.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Courses.API.IntegrationTests;

public class CreateCourseTests
{
    private readonly HttpClient _client;

    public CreateCourseTests()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // ... Configure test services
            });

        _client = application.CreateClient();
    }

    [Fact]
    public async Task ShouldCreateCourse()
    {
        var response = await _client.PostAsJsonAsync("api/courses",
            new CreateCourseRequest(
                "Sanjay Testing",
                TimeSpan.FromHours(1),
                new[] { "ASP.NET Core" },
                false,
                "Sanjay",
                "Pluralsight",
                "",
                "",
                new CreateCourseRequestEntry[]
                {
                    new("One", TimeSpan.FromHours(1), 1, string.Empty)
                })
        );

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}