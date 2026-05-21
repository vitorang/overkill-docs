using OverkillDocs.Core.DTOs.Document.Fragment;
using OverkillDocs.Core.Enums;

namespace OverkillDocs.Core.DTOs.Document;

public sealed record DocumentDetailDto(
    string Title,
    string HashId,
    DocumentType Type,
    DocumentFragmentDto[] Fragments,
    DateTime UpdatedAt
);
