using HashidsNet;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.Entities.Document;

namespace OverkillDocs.Core.Extensions;

public static class DocumentExtensions
{
    public static DocumentSummaryDto ToSummaryDto(this Document document, IHashids hashids)
    {
        return new(
            HashId: hashids.Encode(document.Id),
            Title: document.Title,
            Type: document.Type
        );
    }

    public static DocumentDetailDto ToDetailDto(this Document document, IHashids hashids)
    {
        return new(
            HashId: hashids.Encode(document.Id),
            Title: document.Title,
            Type: document.Type,
            Fragments: [.. document.Fragments.Select(e => e.ToDto(hashids)).OrderBy(e => e.Order)],
            UpdatedAt: document.UpdatedAt
        );
    }

    public static Document WithoutRelationships(this Document document)
    {
        return new Document
        {
            CreatedAt = document.CreatedAt,
            Fragments = [],
            Id = document.Id,
            Title = document.Title,
            Type = document.Type,
            UpdatedAt = document.UpdatedAt,
        };
    }
}
