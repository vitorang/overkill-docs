namespace OverkillDocs.Core.States
{
    public class EditorState
    {
        public string EditorId { get => ConnectionId; }
        public required string ConnectionId { get; set; }
        public required string UserHashId { get; set; }
        public string? DocumentHashId { get; set; }
        public string? FragmentHashId { get; set; }
    }
}
