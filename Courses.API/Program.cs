using Courses.API.DependencyInjection;

namespace Courses.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        // Add services to the container.
        builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSpaStaticFiles(options => { options.RootPath = "dist"; });

        builder.ConfigureInfrastructureDependencies(builder.Configuration);

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.ConfigureHttpPipeline();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseSpaStaticFiles();

        app.UseSpa(spaBuilder =>
        {
            if (app.Environment.IsDevelopment()) spaBuilder.UseProxyToSpaDevelopmentServer("http://127.0.0.1:5174/");
        });

        app.MapControllers();

        app.Run();
    }
}