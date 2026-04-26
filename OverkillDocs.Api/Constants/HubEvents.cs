namespace OverkillDocs.Api.Constants;

public static class HubEvents
{
    public static class Chat
    {
        public const string MessageReceived = "Chat:OnMessageReceived";
        public const string RecentMessagesReceived = "Chat:OnRecentMessagesReceived";
    }

    public static class Document
    {
        public const string DocumentStructureChanged = "OnDocumentStructureChanged";
        public const string FragmentContentChanged = "OnFragmentContentChanged";
        public const string FragmentLockChanged = "OnFragmentLockChanged";
    }
}
