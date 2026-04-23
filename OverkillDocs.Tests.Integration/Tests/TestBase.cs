using OverkillDocs.Tests.Integration.Fixtures;

namespace OverkillDocs.Tests.Integration.Tests
{
    public abstract class TestBase(TestFactory factory) : IClassFixture<TestFactory>, IAsyncLifetime
    {
        protected readonly HttpClient httpClient = factory.CreateClient();

        protected async Task ExecuteInScope<T>(Func<T, Task> action) where T : notnull
            => await factory.ExecuteInScope(action);

        public virtual async Task InitializeAsync()
        {
            await factory.ResetDatabaseAsync();
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;
    }
}
