namespace OverkillDocs.Core.Interfaces
{
    public interface IAppCache<T>
    {
        public Task<T?> Get(int id, CancellationToken ct, Func<Task<T?>>? onCacheMiss = null);
        public Task<T?> Get(string id, CancellationToken ct, Func<Task<T?>>? onCacheMiss = null);
        public Task Set(T value, CancellationToken ct);
        public Task Remove(T value, CancellationToken ct);
        public Task RemoveById(string id, CancellationToken ct);
    }
}
