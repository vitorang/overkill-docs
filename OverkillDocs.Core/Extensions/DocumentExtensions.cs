using HashidsNet;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.Entities.Document;

namespace OverkillDocs.Core.Extensions;

public static class DocumentExtensions
{
    public static DocumentDto ToDto(this Document document, IHashids hashids)
    {
        return new(
            HashId: hashids.Encode(document.Id),
            Title: document.Title,
            Type: document.Type
        );
    }
}
