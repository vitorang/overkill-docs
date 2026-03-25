using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OverkillDocs.Infrastructure.Data;

Console.WriteLine("OverkillDocs.Migrator Iniciado");

var builder = Host.CreateApplicationBuilder(args);

bool useSqlite = builder.Configuration.GetValue<bool>("FeatureFlags:UseSqlite");

string? envProvider = Environment.GetEnvironmentVariable("EF_PROVIDER");
if (envProvider == "Sqlite")
    useSqlite = true;
if (envProvider == "SqlServer")
    useSqlite = false;

string connectionString = useSqlite
    ? builder.Configuration.GetConnectionString("Sqlite")!
    : builder.Configuration.GetConnectionString("SqlServer")!;


builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (useSqlite)
    {
        options.UseSqlite(connectionString,
            x => x.MigrationsAssembly("OverkillDocs.Migrator.Sqlite"));
    }
    else
    {
        options.UseSqlServer(connectionString,
            x => x.MigrationsAssembly("OverkillDocs.Migrator.SqlServer"));
    }
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