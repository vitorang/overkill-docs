using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OverkillDocs.Infrastructure.Data;

Console.WriteLine("OverkillDocs.Migrator Iniciado");

var builder = Host.CreateApplicationBuilder(args);


string entityProviderConfig = Environment.GetEnvironmentVariable("EF_PROVIDER")
    ?? builder.Configuration.GetValue<string>("FeatureFlags:Database")
    ?? throw new Exception("Banco de dados não definido");

var entityProvider = entityProviderConfig switch
{
    "sqlite" => EntityProvider.Sqlite,
    "sqlserver" => EntityProvider.SqlServer,
    "postgres" => EntityProvider.Postgres,
    _ => EntityProvider.Unknown
};

builder.Services.AddDbContext<AppDbContext>(options =>
{
    _ = entityProvider switch
    {
        EntityProvider.Sqlite => options.UseSqlite(
            builder.Configuration.GetConnectionString("Sqlite"),
            x => x.MigrationsAssembly("OverkillDocs.Migrator.Sqlite")),

        EntityProvider.SqlServer => options.UseSqlServer(
            builder.Configuration.GetConnectionString("SqlServerAdmin"),
            x => x.MigrationsAssembly("OverkillDocs.Migrator.SqlServer")),

        EntityProvider.Postgres => options.UseNpgsql(
            builder.Configuration.GetConnectionString("PostgresAdmin"),
            x => x.MigrationsAssembly("OverkillDocs.Migrator.Postgres")),

        _ => throw new Exception("Provedor de banco de dados não mapeado")
    };
});


using IHost host = builder.Build();
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro na migração: {ex.Message}");
        Environment.Exit(1);
    }
}

Console.WriteLine("OverkillDocs.Migrator Finalizado");

enum EntityProvider
{
    Sqlite,
    SqlServer,
    Postgres,
    Unknown
}
