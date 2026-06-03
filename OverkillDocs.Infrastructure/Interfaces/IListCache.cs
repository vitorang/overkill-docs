using System.Collections.Immutable;

namespace OverkillDocs.Infrastructure.Interfaces;

internal interface IListCache<T>
{
    Task Append(T value);
    Task<ImmutableArray<T>> Get();
}
