using OverkillDocs.Core.Enums;

namespace OverkillDocs.Core.Entities.Document;

public sealed class DocumentFragment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Order { get; set; }
    public DocumentFragmentType Type { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int DocumentId { get; set; }
    public required Document Document { get; set; }
}
