namespace OverkillDocs.Core.Interfaces;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct);
}
