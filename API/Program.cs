using System.Text.Json.Serialization;
using CourseModule;
using Courses.API.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WatchModule;

namespace Courses.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        // Add services to the container.
        builder.Services.AddControllers()
            // https://stackoverflow.com/a/55541764
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .AddApplicationPart(typeof(CourseModule.CourseModule).Assembly)
            .AddApplicationPart(typeof(WatchModule.WatchModule).Assembly)
            ;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(e =>
        {
            e.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);

            e.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Courses Api", Version = "v1", Description = "Api to manage courses and Youtube playlists", License = new OpenApiLicense
                {
                    Name = "Apache 2.0",
                    Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                }
            });
            e.EnableAnnotations();
        });
        builder.Services
            .RegisterCourseModule(builder.Configuration)
            .RegisterWatchModule(builder.Configuration)
            ;

        builder.ConfigureInfrastructureDependencies(builder.Configuration);

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.ConfigureHttpPipeline();

        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}