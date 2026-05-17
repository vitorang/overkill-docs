namespace OverkillDocs.Core.DTOs.Document.Fragment;

public sealed record ArticleMarkdownFragmentDto : DocumentFragmentDto
{
    public required string Text { get; init; }
}
