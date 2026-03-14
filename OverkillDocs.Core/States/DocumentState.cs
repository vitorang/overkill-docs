namespace OverkillDocs.Core.States
{
    public class DocumentState
    {
        public required string DocumentHashId { get; set; }
        public HashSet<DocumentFragmentEdit> FragmentEdits = [];
    
        public record DocumentFragmentEdit(string ConnectionId, string UserHashId, string FragmentHashId);
    }
}
