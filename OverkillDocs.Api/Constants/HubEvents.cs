using OverkillDocs.Api.Hubs;

namespace OverkillDocs.Api.Constants;

public static class HubEvents
{
    public static class Chat
    {
        public const string MessageReceived = $"{MainHub.chatGroup}:OnMessageReceived";
        public const string RecentMessagesReceived = $"{MainHub.chatGroup}:OnRecentMessagesReceived";
    }

    public static class DocumentIndex
    {
        public const string Changed = $"{MainHub.documentIndexGroup}:OnChanged";
    }
}
