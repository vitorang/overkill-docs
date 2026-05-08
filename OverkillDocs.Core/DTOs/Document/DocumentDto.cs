using OverkillDocs.Core.Attributes;
using OverkillDocs.Core.Enums;

namespace OverkillDocs.Core.DTOs.Document;

public sealed record DocumentDto(
    [DocumentName]
    string Title,
    string HashId,
    DocumentType Type
);
