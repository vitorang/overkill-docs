using OverkillDocs.Infrastructure.Data;
using OverkillDocs.Tests.Integration.Fixtures;
using System.Text.Json;
using Xunit.Abstractions;

namespace OverkillDocs.Tests.Integration.Tests
{
    public abstract class TestBase(TestFactory factory, ITestOutputHelper outputHelper) : IClassFixture<TestFactory>, IAsyncLifetime
    {
        private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

        protected readonly HttpClient httpClient = factory.CreateClient();

        protected async Task ExecuteInScope<T>(Func<T, Task> action) where T : notnull
            => await factory.ExecuteInScope(action);

        protected async Task ExecuteAndCommit(Action<AppDbContext> action)
        {
            await ExecuteInScope<AppDbContext>(async db =>
            {
                action(db);
                await db.SaveChangesAsync();
            });
        }

        public virtual async Task InitializeAsync()
        {
            await factory.ResetDatabaseAsync();
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;

        public void LogData(params object?[] items)
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
    }
}
