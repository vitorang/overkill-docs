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

    public static class DocumentView
    {
        public const string ActiveLocksChanged = $"{MainHub.documentViewGroup}:OnActiveLocksChanged";
        public const string DocumentChanged = $"{MainHub.documentViewGroup}:OnDocumentChanged";
        public const string FragmentChanged = $"{MainHub.documentViewGroup}:OnFragmentChanged";
    }
}
