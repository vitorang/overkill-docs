namespace OverkillDocs.Core.Entities.Chat
{
    public class ChatHistory
    {
        public static readonly int DefaultId = 0;
        public int Id { get => DefaultId; }
        private const int limit = 20;

        List<ChatMessage> Messages { get; set; } = [];

        public void Add(ChatMessage message) {

            Messages = [.. Messages.TakeLast(limit - 1), message];
        }

        public IEnumerable<ChatMessage> RecentMessages(TimeSpan lifetime)
        {
            var now = DateTime.UtcNow;
            return Messages.Where(e => (now - e.Timestamp) < lifetime);
        }
    }
}
