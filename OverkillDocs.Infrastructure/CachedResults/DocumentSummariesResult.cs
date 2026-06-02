using OverkillDocs.Core.DTOs.Document;

namespace OverkillDocs.Infrastructure.CachedResults;

internal sealed record DocumentSummariesResult(
    DocumentSummaryDto[] Documents)
{
    public static readonly int DefaultId = 0;
}
