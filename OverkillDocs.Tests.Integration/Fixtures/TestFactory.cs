using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OverkillDocs.Infrastructure.Data;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace OverkillDocs.Tests.Integration.Fixtures
{
    public class TestFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder("postgres:alpine")
            .WithDatabase("OverkillDocsTestDb")
            .WithUsername("sa")
            .WithPassword("P@ssword123")
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

        public async Task ExecuteInScope<T>(Func<T, Task> action)
            where T : notnull
        {
            using var scope = Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<T>();
            await action(service);
        }
    }
}