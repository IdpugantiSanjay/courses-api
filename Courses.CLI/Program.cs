// See https://aka.ms/new-console-template for more information

using Elastic.Apm.NetCoreAll;
using Elastic.Apm.SerilogEnricher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Courses.CLI;

public class Program
{
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .UseAllElasticApm();
    }

    public static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        BuildConfig(builder);

        var configuration = builder.Build();

        var host = CreateHostBuilder(args)
            .UseSerilog((ctx, lc) =>
            {
                lc.ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithElasticApmCorrelationInfo()
                    .WriteTo.Console()
                    .WriteTo.File("/var/log/courses/cli/log.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri(configuration.GetConnectionString("ElasticSearchUrl"))))
                    ;
            })
            .ConfigureServices(s => { s.AddSingleton<IndexCourse>(); })
            .Build();

        var ingest = host.Services.GetService<IndexCourse>()!;
        await ingest.Ingest();
    }

    private static void BuildConfig(IConfigurationBuilder builder)
    {
        var secondaryAppSettingName =
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json";

        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile(
                secondaryAppSettingName,
                false, true)
            .AddEnvironmentVariables()
            ;
    }
}