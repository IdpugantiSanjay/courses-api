// See https://aka.ms/new-console-template for more information

using System.CommandLine;
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
            .ConfigureServices(s =>
            {
                s.AddSingleton<IndexCourse>();
                s.AddSingleton<ListCourses>();
            })
            .Build();

        var rootCommand = new RootCommand("cli to ingest courses");
        var addCommand = new Command("add");
        var listCommand = new Command("list");

        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(listCommand);

        var path = new Argument<DirectoryInfo>(
            "path",
            "course path");

        var author = new Option<string>("--author", () => string.Empty);
        var categories = new Option<string[]>("--categories", Array.Empty<string>);
        var platform = new Option<string>("--platform", () => string.Empty);

        addCommand.AddArgument(path);
        addCommand.AddOption(author);
        addCommand.AddOption(categories);
        addCommand.AddOption(platform);

        listCommand.AddOption(author);
        listCommand.AddOption(categories);
        listCommand.AddOption(platform);

        addCommand.SetHandler(async (pathInput, authorInput, platformInput, categoriesInput) =>
        {
            var ingest = host.Services.GetService<IndexCourse>()!;
            await ingest.Ingest(pathInput, authorInput, platformInput, categoriesInput);
        }, path, author, platform, categories);

        addCommand.AddValidator(r =>
        {
            var pathValue = r.GetValueForArgument(path);
            if (pathValue is null || !pathValue.Exists) r.ErrorMessage = "The path provided is invalid.";
        });

        listCommand.SetHandler(async () =>
        {
            var list = host.Services.GetService<ListCourses>()!;
            await list.List();
        });

        await rootCommand.InvokeAsync(args);
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