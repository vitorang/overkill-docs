using OverkillDocs.Core.Entities.Identity;

namespace OverkillDocs.Core.Entities.Security
{
    public class UserSession
    {
        public int Id { get; set; }

        public string Token { get; init; } = Ulid.NewUlid().ToString();
        public required string UserAgent { get; init; }
        
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
        
        public int UserId { get; set; }
        public required User User { get; set; }
    }
}
