using System.Security.Cryptography;

namespace OverkillDocs.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; init; } = GenerateSalt();
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        private static string GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
    }
}