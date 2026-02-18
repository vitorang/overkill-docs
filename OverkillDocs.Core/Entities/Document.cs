namespace OverkillDocs.Core.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedById { get; set; }
        public required User CreatedBy { get; set; }

        public ICollection<DocumentFragment> Fragments { get; set; } = [];
    }
}
