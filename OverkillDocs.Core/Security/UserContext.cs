namespace OverkillDocs.Core.Security
{
    public class UserContext
    {
        public int UserId { get => Identity.UserId; }
        public string Username { get => Identity.Username; }
        public string Token { get => Identity.Token; }
        public bool IsAuthorized => Identity != Anonymous;

        private static readonly UserIdentity Anonymous = new(0, "Anonymous", string.Empty);
        public UserIdentity Identity { get; set; } = Anonymous;

        public record UserIdentity(int UserId, string Username, string Token);
    }
}
