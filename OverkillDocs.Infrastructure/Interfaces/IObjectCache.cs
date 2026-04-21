namespace OverkillDocs.Infrastructure.Interfaces
{
    public interface IObjectCache<T>
    {
        public Task<T?> Get(int id, Func<Task<T?>>? onCacheMiss = null);
        public Task<T?> Get(string id, Func<Task<T?>>? onCacheMiss = null);
        public Task Set(T value);
        public Task Remove(T value);
        public Task RemoveAll(IEnumerable<T> values);
        public Task RemoveById(string id);
    }
}
