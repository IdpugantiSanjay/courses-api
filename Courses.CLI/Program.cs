// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Courses.CLI;

public class Program
{
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder().UseSerilog();
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
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithProperty("Project", "Courses.Cli")
                    .WriteTo.Console(LogEventLevel.Error)
                    .AuditTo.Seq(configuration.GetConnectionString("Seq")!, apiKey: configuration.GetSection("Seq").GetValue<string>("API_KEY"))
                    ;
            })
            .ConfigureServices(s =>
            {
                s.AddSingleton<IndexCourse>();
                s.AddSingleton<CleanCourseDirectory>();
                s.AddSingleton<ListCourses>();
                s.AddSingleton<DeleteCourse>();
                s.AddSingleton<GetCourse>();
                s.AddSingleton<Playlist>();
                s.AddSingleton<HttpInterceptor>();

                void ConfigureClient(HttpClient client)
                {
                    var api = configuration.GetValue<string>("BackendApi")!;
                    client.BaseAddress = new Uri($"{api}/api/v1/Courses/");
                }

                s.AddHttpClient<IndexCourse>(ConfigureClient);
                s.AddHttpClient<ListCourses>(ConfigureClient);
                s.AddHttpClient<DeleteCourse>(ConfigureClient);
                s.AddHttpClient<GetCourse>(ConfigureClient);
                s.AddHttpClient<Playlist>(ConfigureClient);
            })
            .Build();

        var rootCommand = new RootCommand("cli to ingest courses");
        var addCommand = new Command("add");
        var listCommand = new Command("list");
        var deleteCommand = new Command("delete");
        var getCommand = new Command("get");
        var playlistCommand = new Command("playlist");

        rootCommand.AddCommand(addCommand);
        rootCommand.AddCommand(listCommand);
        rootCommand.AddCommand(deleteCommand);
        rootCommand.AddCommand(getCommand);
        rootCommand.AddCommand(playlistCommand);

        var path = new Argument<DirectoryInfo>(
            "path",
            "course path");

        var categories = new Option<string[]>("--categories", Array.Empty<string>);
        var playlistId = new Argument<string>("id", "Id of youtube playlist");

        // var author = new Option<string>("--author", () => string.Empty);
        //
        // var platform = new Option<string>("--platform", () => string.Empty);

        playlistCommand.AddArgument(playlistId);

        addCommand.AddArgument(path);
        addCommand.AddOption(categories);

        listCommand.AddOption(categories);

        var idArgument = new Argument<int>("id");
        getCommand.AddArgument(idArgument);
        deleteCommand.AddArgument(idArgument);

        addCommand.SetHandler(async (pathInput, categoriesInput) =>
        {
            var ingest = host.Services.GetService<IndexCourse>()!;
            var cleaner = host.Services.GetService<CleanCourseDirectory>()!;
            var cleaned = cleaner.Clean(pathInput);
            await ingest.Ingest(cleaned, categoriesInput);
        }, path, categories);

        playlistCommand.SetHandler(async playlistIdInput =>
        {
            var playlist = host.Services.GetService<Playlist>()!;
            await playlist.Index(playlistIdInput, CancellationToken.None);
        }, playlistId);

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