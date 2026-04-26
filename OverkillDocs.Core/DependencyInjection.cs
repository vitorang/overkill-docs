using Microsoft.Extensions.DependencyInjection;
using OverkillDocs.Core.Security;

namespace OverkillDocs.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddOkdCore(this IServiceCollection services)
    {
        services.Scan(scan => scan
        .FromAssembliesOf(typeof(DependencyInjection))
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")), publicOnly: false)
        .AsImplementedInterfaces()
        .WithScopedLifetime());

        services.AddScoped<UserContext>();
        return services;
    }
}
