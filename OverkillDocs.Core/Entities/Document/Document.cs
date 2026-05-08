using OverkillDocs.Core.Enums;

namespace OverkillDocs.Core.Entities.Document;

public sealed class Document
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required DocumentType Type { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<DocumentFragment> Fragments { get; set; } = [];
}
