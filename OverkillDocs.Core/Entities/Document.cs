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
        public User CreatedBy { get; set; } = null!;

        public ICollection<DocumentFragment> Fragments { get; set; } = [];
    }
}
