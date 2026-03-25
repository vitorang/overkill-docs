using System.Collections.Immutable;

namespace OverkillDocs.Core.Interfaces
{
    public interface IListCache<T>
    {
        public Task Append(T value);
        public Task<ImmutableArray<T>> Get();
    }
}
