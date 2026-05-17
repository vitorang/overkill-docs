using OverkillDocs.Core.DTOs.Document;

namespace OverkillDocs.Infrastructure.CachedResults;

internal sealed record DocumentListResult(DocumentSummaryDto[] Documents);
