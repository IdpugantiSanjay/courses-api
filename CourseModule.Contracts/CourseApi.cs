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
    Task<CourseResponse.Default> Get(int id);

    [Get($$"""{{Constants.CurrentVersionCoursesApiBasePath}}/{id}?view=entries""")]
    Task<CourseResponse.WithEntries> GetWithEntries(int id);

    [Post(Constants.CurrentVersionCoursesApiBasePath)]
    Task<int> Create([Body] CreateRequestBody.Default request);

    [Post(Constants.CurrentVersionCoursesApiBasePath)]
    Task<int> Create([Body] CreateRequestBody.Playlist request);

    [Get($"{Constants.CurrentVersionCoursesApiBasePath}?view=entries")]
    Task<ListResponse<CourseResponse.WithEntries>> ListWithEntries();

    [Get($"{Constants.CurrentVersionCoursesApiBasePath}?view=default")]
    Task<ListResponse<CourseResponse.Default>> List();

    [Get($$"""{{Constants.CurrentVersionCoursesApiBasePath}}/{id}""")]
    Task Delete(int id);
}