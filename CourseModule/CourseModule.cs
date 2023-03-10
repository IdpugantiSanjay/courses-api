using CourseModule.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseModule;

public static class CourseModule
{
    public static IServiceCollection RegisterCourseModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddCourseModule()
                .AddDbContext<CourseDbContext>(options => { options.UseNpgsql(configuration.GetConnectionString("Default")); })
            ;
    }
}