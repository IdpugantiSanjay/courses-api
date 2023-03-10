using CourseModule.Database;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using WatchModule.Database;

namespace Watch.IntegrationTests;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor != null) services.Remove(descriptor);
    }

    public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
    {
        var serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<T>();
        context.Database.EnsureCreated();
    }
}

[UsedImplicitly]
public class IntegrationTestFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private const string ConnectionString = "Host=192.168.29.157;Username=postgres;Password=postgres;Database=courses_integration_test";
    private Respawner _respawner = null!;

    public async Task InitializeAsync()
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
        var respawnerOptions = new RespawnerOptions { DbAdapter = DbAdapter.Postgres };
        _respawner = await Respawner.CreateAsync(conn, respawnerOptions);
        await _respawner.ResetAsync(conn);
    }

    public new Task DisposeAsync()
    {
        return Task.FromResult(Task.CompletedTask);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<CourseDbContext>();
            services.AddDbContext<CourseDbContext>(options => { options.UseNpgsql(ConnectionString); });
            services.EnsureDbCreated<CourseDbContext>();

            services.RemoveDbContext<WatchDbContext>();
            services.AddDbContext<WatchDbContext>(options => { options.UseNpgsql(ConnectionString); });
            services.EnsureDbCreated<WatchDbContext>();
        });
    }
}