namespace OverkillDocs.Core.DTOs.Document.Fragment;

public sealed record ArticleImageFragmentDto : DocumentFragmentDto
{
    public required string Url { get; init; }
    public required string Alt { get; init; }
}
