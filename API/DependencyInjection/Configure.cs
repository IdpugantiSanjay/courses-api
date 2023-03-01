using System.Net.Mime;
using System.Reflection;
using Elastic.CommonSchema.Serilog;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

// using Elastic.Apm.NetCoreAll;

namespace Courses.API.DependencyInjection;

public static class ConfigurationKeys
{
    public const string PostgresConnectionStringKey = "Postgres";
    public const string LogPathKey = "LogToPath";
    public const string ElasticSearchConnectionStringKey = "ElasticSearchUrl";
}

public static class Configure
{
    public static void ConfigureInfrastructureDependencies(this WebApplicationBuilder builder,
        IConfigurationRoot configuration)
    {
        builder.ConfigureSerilog(configuration);
        builder.Services.AddCommonLibraries(configuration);
    }

    private static void ConfigureSerilog(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        // Serilog.Debugging.SelfLog.Enable(Console.Error);

        // var elasticSearchServerUrl = configuration.GetConnectionString(ConfigurationKeys.ElasticSearchConnectionString);
        // var indexFormat =
        //         $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{builder.Environment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
        //     ;
        //
        //     .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
        //         new Uri(elasticSearchServerUrl)
        //     )
        //     {
        //         AutoRegisterTemplate = true,
        //         AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
        //         IndexFormat = indexFormat,
        //         CustomFormatter = new EcsTextFormatter()
        //     }
        // )

        builder.Host.UseSerilog((_, lc) =>
            {
                lc
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithMachineName()
                    .Enrich.WithCorrelationIdHeader()
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    // .Enrich.WithElasticApmCorrelationInfo()
                    .ReadFrom.Configuration(configuration)
                    .WriteTo.File(new EcsTextFormatter(), configuration.GetValue<string>(ConfigurationKeys.LogPathKey)!,
                        rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.FromSeconds(5))
                    .WriteTo.Console()
                    .WriteTo.Seq(configuration.GetConnectionString("Seq")!, apiKey: configuration.GetSection("Seq").GetValue<string>("API_KEY"))
                    ;
            })
            // .UseAllElasticApm()
            ;
    }

    private static void AddCommonLibraries(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        config.Scan(Assembly.GetExecutingAssembly());

        var configValues = new
        {
            ElasticSearchConnectionString =
                configuration.GetConnectionString(ConfigurationKeys.ElasticSearchConnectionStringKey),
            NpgSqlConnectionString = configuration.GetConnectionString(ConfigurationKeys.PostgresConnectionStringKey)
        };

        serviceCollection
            .AddSingleton(config)
            // .AddScoped<IMapper, ServiceMapper>()
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            // .AddMediatR(typeof(CoursesController))
            .AddHealthChecks()
            // .AddElasticsearch(configValues.ElasticSearchConnectionString)
            .AddNpgSql(configValues.NpgSqlConnectionString)
            ;

        // serviceCollection.AddDbContext<AppDbContext>(options =>
        //     options.UseNpgsql(configValues.NpgSqlConnectionString));
    }

    public static void ConfigureHttpPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseCors(cp => cp.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

        app.MapHealthChecks("/healthz", new HealthCheckOptions
        {
            ResultStatusCodes = new Dictionary<HealthStatus, int>
            {
                { HealthStatus.Unhealthy, 500 },
                { HealthStatus.Healthy, 200 },
                { HealthStatus.Degraded, 500 }
            },
            ResponseWriter = async (context, report) =>
            {
                var result = JsonConvert.SerializeObject(
                    new
                    {
                        Name = "Courses API",
                        Status = report.Status.ToString(),
                        Duration = report.TotalDuration,
                        Info = report.Entries.Select(e => new
                        {
                            e.Key,
                            e.Value.Description,
                            e.Value.Duration,
                            Status = Enum.GetName(typeof(HealthStatus), e.Value.Status),
                            Error = e.Value.Exception?.Message
                        }).ToList()
                    }, Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(result);
            }
        });
        app.UseSerilogRequestLogging();
    }
}