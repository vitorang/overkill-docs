using OverkillDocs.Core.Enums;
using System.Text.Json.Serialization;

namespace OverkillDocs.Core.DTOs.Document.Fragment;

[JsonDerivedType(typeof(ArticleMarkdownFragmentDto), typeDiscriminator: (int)DocumentFragmentType.Markdown)]
[JsonDerivedType(typeof(ArticleImageFragmentDto), typeDiscriminator: (int)DocumentFragmentType.Image)]
[JsonDerivedType(typeof(ArticleEmbedFragmentDto), typeDiscriminator: (int)DocumentFragmentType.Embed)]
public abstract record DocumentFragmentDto
{
    public required string HashId { get; init; }
    public required string DocumentHashId { get; init; }
    public DocumentFragmentType Type { get; init; }
    public double Order { get; init; }
    public DateTime UpdatedAt { get; set; }
}
