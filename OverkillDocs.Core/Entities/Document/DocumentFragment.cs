using OverkillDocs.Core.Entities.Identity;
using OverkillDocs.Core.Enums;

namespace OverkillDocs.Core.Entities.Document;

public class DocumentFragment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Order { get; set; }
    public DocumentFragmentType Type { get; set; } = DocumentFragmentType.PlainText;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int DocumentId { get; set; }
    public required Document Document { get; set; }

    public int EditedById { get; set; }
    public required User EditedBy { get; set; }
}
