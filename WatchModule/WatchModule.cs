using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WatchModule.Database;

namespace WatchModule;

public static class WatchModule
{
    public static IServiceCollection RegisterWatchModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddWatchModule()
                .AddDbContext<WatchDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")))
            ;
    }
}