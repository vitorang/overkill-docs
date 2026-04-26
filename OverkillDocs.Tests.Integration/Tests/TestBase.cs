using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Tests.Integration.Fakers.Entities.Identity;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit.Abstractions;

namespace OverkillDocs.Tests.Integration.Tests;

public abstract class TestBase(TestFactory factory, ITestOutputHelper outputHelper) : IClassFixture<TestFactory>, IAsyncLifetime
{
    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    protected readonly HttpClient httpClient = factory.CreateClient();

    public virtual async Task InitializeAsync()
    {
        await factory.ResetDatabaseAsync();

        if (Require<IMemoryCache>() is MemoryCache cache)
            cache.Clear();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    private async Task Execute<T>(Func<T, Task> action)
        where T : notnull
    {
        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();
        await action(service);
    }

    protected async Task Execute(Func<AppDbContext, Task> action)
    {
        await Execute<AppDbContext>(async db =>
        {
            await action(db);
            await db.SaveChangesAsync();
        });
    }

    protected async Task ExecuteAndCommit(Action<AppDbContext> action)
    {
        await Execute<AppDbContext>(async db =>
        {
            action(db);
            await db.SaveChangesAsync();
        });
    }

    protected void LogData(params object?[] items)
    {
        int index = 0;
        foreach (var item in items)
        {
            var typeName = item?.GetType().Name ?? "NULL";
            var json = JsonSerializer.Serialize(item, jsonOptions);

            outputHelper.WriteLine($"#{index}: {typeName}\n{json}\n");
            index++;
        }
    }

    protected async Task<UserSession> LoginAs(User user)
    {
        if (user.Id == 0)
            await ExecuteAndCommit(db => db.Users.Add(user));

        var session = new UserSessionFaker(user).Generate();
        await ExecuteAndCommit(db =>
        {
            db.Users.Attach(user);
            db.UserSessions.Add(session);
        });

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", session.Token);

        return session;
    }

    protected T Require<T>() where T : notnull
    {
        if (typeof(T) == typeof(AppDbContext))
            throw new InvalidOperationException("Require<AppDbContext>() não é suportado. Use Execute() ou ExecuteAndCommit().");

        return factory.Services.GetRequiredService<T>();
    }
}
