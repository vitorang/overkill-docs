namespace OverkillDocs.Core.Security
{
    public class UserContext
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsAuthorized => UserId > 0;
    }
}
