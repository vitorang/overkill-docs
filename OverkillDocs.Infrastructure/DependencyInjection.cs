using Microsoft.Extensions.DependencyInjection;
using OverkillDocs.Core.Interfaces;
using OverkillDocs.Infrastructure.Cache.Memory;
using OverkillDocs.Infrastructure.Cache.Redis;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Infrastructure.Interfaces;
using StackExchange.Redis;

namespace OverkillDocs.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOkdInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Scan(scan => scan
            .FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository") || type.Name.EndsWith("Service")), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddOkdCache(this IServiceCollection services, bool useRedis, string redisConnection)
    {

        if (useRedis)
        {
            services.AddSingleton(typeof(IObjectCache<>), typeof(RedisObjectCache<>));
            services.AddSingleton(typeof(IListCache<>), typeof(RedisListCache<>));

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnection));
        }
        else
        {
            services.AddSingleton(typeof(IObjectCache<>), typeof(MemoryObjectCache<>));
            services.AddSingleton(typeof(IListCache<>), typeof(MemoryListCache<>));

            services.AddMemoryCache();
        }

        return services;
    }
}
