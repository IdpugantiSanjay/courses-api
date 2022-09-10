// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Courses.CLI;

public class Program
{
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                .UseSerilog()
            // .UseAllElasticApm()
            ;
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
                    // .Enrich.WithElasticApmCorrelationInfo()
                    .WriteTo.Console()
                    .WriteTo.File("/var/log/courses/cli/log.txt", rollingInterval: RollingInterval.Day)
                    // .WriteTo.Elasticsearch(
                    //     new ElasticsearchSinkOptions(new Uri(configuration.GetConnectionString("ElasticSearchUrl"))))
                    ;
            })
            .ConfigureServices(s =>
            {
                s.AddSingleton<IndexCourse>();
                s.AddSingleton<CleanCourseDirectory>();
                s.AddSingleton<ListCourses>();
                s.AddSingleton<DeleteCourse>();
                s.AddSingleton<GetCourse>();
            })
            .Build();

        var rootCommand = new RootCommand("cli to ingest courses");
        var addCommand = new Command("add");
        var listCommand = new Command("list");
        var deleteCommand = new Command("delete");
        var getCommand = new Command("get");

        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(listCommand);
        rootCommand.AddCommand(deleteCommand);
        rootCommand.AddCommand(getCommand);

        var path = new Argument<DirectoryInfo>(
            "path",
            "course path");

        var idArgument = new Argument<int>("id");
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

        getCommand.AddArgument(idArgument);
        deleteCommand.AddArgument(idArgument);

        addCommand.SetHandler(async (pathInput, authorInput, platformInput, categoriesInput) =>
        {
            var ingest = host.Services.GetService<IndexCourse>()!;
            var cleaner = host.Services.GetService<CleanCourseDirectory>()!;
            var cleaned = cleaner.Clean(pathInput);
            await ingest.Ingest(cleaned, authorInput, platformInput, categoriesInput);
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

        deleteCommand.SetHandler(async id =>
        {
            var delete = host.Services.GetService<DeleteCourse>()!;
            await delete.Delete(id);
        }, idArgument);

        getCommand.SetHandler(async id =>
        {
            var get = host.Services.GetService<GetCourse>()!;
            await get.Get(id);
        }, idArgument);

        await rootCommand.InvokeAsync(args);
    }

    private static void BuildConfig(IConfigurationBuilder builder)
    {
        var secondaryAppSettingName =
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json";

        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile(
                secondaryAppSettingName,
                true, true)
            .AddEnvironmentVariables()
            ;
    }
}