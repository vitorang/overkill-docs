using System.Collections.Immutable;

namespace OverkillDocs.Core.Interfaces
{
    public interface IListCache<T>
    {
        public Task Append(T value, CancellationToken ct);
        public Task<ImmutableList<T>> Get(CancellationToken ct);
    }
}
