using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Infrastructure.Data;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace OverkillDocs.Tests.Integration.Fixtures;

public class TestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly string DbName = Environment.GetEnvironmentVariable("TEST_DB_NAME") ?? "OverkillDocsTestDb";
    private static readonly string DbUser = Environment.GetEnvironmentVariable("TEST_DB_USER") ?? "sa";
    private static readonly string DbPass = Environment.GetEnvironmentVariable("TEST_DB_PASS") ?? "P@ssword123";

    private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder("postgres:alpine")
        .WithDatabase(DbName)
        .WithUsername(DbUser)
        .WithPassword(DbPass)
        .Build();

    private Respawner respawner = default!;
    private DbConnection dbConnection = default!;

    public async Task InitializeAsync()
    {
        await dbContainer.StartAsync();

        dbConnection = new NpgsqlConnection(dbContainer.GetConnectionString());
        await dbConnection.OpenAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        respawner = await Respawner.CreateAsync(dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"]
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.UseSetting("ConnectionStrings:Postgres", dbContainer.GetConnectionString());

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var config = configBuilder.Build();
            ValidateSettings(config);
        });

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(dbContainer.GetConnectionString());
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await respawner.ResetAsync(dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await dbConnection.CloseAsync();
        await dbContainer.StopAsync();
    }

    private static void ValidateSettings(IConfigurationRoot config)
    {
        var useRedis = config.GetValue<bool>("FeatureFlags:UseRedis");
        if (useRedis)
            throw new InvalidOperationException("Somente FeatureFlags:UseRedis=False é suportado.");

        var database = config.GetValue<string>("FeatureFlags:Database");
        if (database != "postgres")
            throw new InvalidOperationException("Somente FeatureFlags:Database=postgres é suportado.");
    }
}
