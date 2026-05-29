namespace OverkillDocs.Infrastructure.Interfaces;

internal interface IObjectCache<T>
{
    public Task<T?> Get(int id, Func<Task<T?>>? onCacheMiss = null);
    public Task<T?> Get(string id, Func<Task<T?>>? onCacheMiss = null);
    public Task<T[]> GetAll(int[] ids);
    public Task<T[]> GetAll(string[] ids);
    public Task<bool> CreateOrRenew(T value);
    public Task Set(T value);
    public Task Remove(T value, bool ifEquals = false);
    public Task RemoveAll(IEnumerable<T> values);
    public Task RemoveById(int id);
    public Task RemoveById(string id);
    public string IdFrom(T value);
}
