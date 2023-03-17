using Contracts;
using Refit;

namespace CourseModule.Contracts;

file static class Constants
{
    private const string Version = "v1";

    public const string CurrentVersionCoursesApiBasePath = $"/api/{Version}/courses";
}

public interface ICourseApi
{
    [Get($$"""{{Constants.CurrentVersionCoursesApiBasePath}}/{id}?view=default""")]
    Task<CourseResponse.Default> Get(int id, [Header("x-correlation-id")] string correlationId);

    [Get($$"""{{Constants.CurrentVersionCoursesApiBasePath}}/{id}?view=entries""")]
    Task<CourseResponse.WithEntries> GetWithEntries(int id, [Header("x-correlation-id")] string correlationId);

    [Post(Constants.CurrentVersionCoursesApiBasePath)]
    Task<int> Create([Body] CreateRequestBody.Default request, [Header("x-correlation-id")] string correlationId);

    [Post(Constants.CurrentVersionCoursesApiBasePath)]
    Task<int> Create([Body] CreateRequestBody.Playlist request, [Header("x-correlation-id")] string correlationId);

    [Get($"{Constants.CurrentVersionCoursesApiBasePath}")]
    Task<ListResponse<CourseResponse>> List(CourseView view, [Header("x-correlation-id")] string correlationId);

    [Get($$"""{{Constants.CurrentVersionCoursesApiBasePath}}/{id}""")]
    Task Delete(int id, [Header("x-correlation-id")] string correlationId);
}