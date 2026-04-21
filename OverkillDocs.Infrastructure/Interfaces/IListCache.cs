using System.Collections.Immutable;

namespace OverkillDocs.Infrastructure.Interfaces
{
    public interface IListCache<T>
    {
        public Task Append(T value);
        public Task<ImmutableArray<T>> Get();
    }
}
