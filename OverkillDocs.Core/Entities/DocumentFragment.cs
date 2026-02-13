using OverkillDocs.Core.Enums;

namespace OverkillDocs.Core.Entities
{
    public class DocumentFragment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public double Order { get; set; }
        public DocumentFragmentType Type { get; set; } = DocumentFragmentType.PlainText;

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int DocumentId { get; set; }
        public Document Document { get; set; } = null!;

        public int EditedById { get; set; }
        public User EditedBy { get; set; } = null!;
    }
}
