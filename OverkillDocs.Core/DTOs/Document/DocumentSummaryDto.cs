using OverkillDocs.Core.Attributes;
using OverkillDocs.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Document;

public sealed record DocumentSummaryDto(
    [DocumentTitle]
    string Title,
    [Required]
    string HashId,
    DocumentType Type
);
