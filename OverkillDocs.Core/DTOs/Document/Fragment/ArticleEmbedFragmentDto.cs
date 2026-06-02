namespace OverkillDocs.Core.DTOs.Document.Fragment;

public sealed record ArticleEmbedFragmentDto : DocumentFragmentDto
{
    public required string Url { get; init; }
}
