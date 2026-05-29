using System.Collections.Immutable;

namespace OverkillDocs.Infrastructure.Interfaces;

internal interface IListCache<T>
{
    public Task Append(T value);
    public Task<ImmutableArray<T>> Get();
}
