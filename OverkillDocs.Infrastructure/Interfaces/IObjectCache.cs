namespace OverkillDocs.Infrastructure.Interfaces;

internal interface IObjectCache<T>
{
    Task<T?> Get(int id, Func<Task<T?>>? onCacheMiss = null);
    Task<T?> Get(string id, Func<Task<T?>>? onCacheMiss = null);
    Task<T[]> GetAll(int[] ids);
    Task<T[]> GetAll(string[] ids);
    Task<bool> CreateOrRenew(T value);
    Task Set(T value);
    Task Remove(T value);
    Task RemoveIfEquals(T value);
    Task RemoveAll(IEnumerable<T> values);
    Task RemoveById(int id);
    Task RemoveById(string id);
    string IdFrom(T value);
    void MarkAsInvalid(int id);
    void MarkAsInvalid(string id);
    void MarkAsInvalid(T value);
}
