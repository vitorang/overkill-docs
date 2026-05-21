using HashidsNet;
using OverkillDocs.Core.DTOs.Document.Fragment;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Enums;
using System.Text.Json;

namespace OverkillDocs.Core.Extensions;

public static class DocumentFragmentExtensions
{
    private static readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);

    public static DocumentFragmentDto ToDto(this DocumentFragment fragment, IHashids hashids)
    {
        var content = JsonSerializer.Deserialize<Dictionary<string, string>>(fragment.Content)!;


        if (fragment.Type == DocumentFragmentType.Markdown)
            return new ArticleMarkdownFragmentDto
            {
                HashId = hashids.Encode(fragment.Id),
                DocumentHashId = hashids.Encode(fragment.DocumentId),
                Order = fragment.Order,
                Type = fragment.Type,
                Text = content["text"]
            };

        if (fragment.Type == DocumentFragmentType.Image)
            return new ArticleImageFragmentDto
            {
                HashId = hashids.Encode(fragment.Id),
                DocumentHashId = hashids.Encode(fragment.DocumentId),
                Order = fragment.Order,
                Type = fragment.Type,
                Url = content["url"],
                Alt = content["alt"]
            };

        if (fragment.Type == DocumentFragmentType.Embed)
            return new ArticleEmbedFragmentDto
            {
                HashId = hashids.Encode(fragment.Id),
                DocumentHashId = hashids.Encode(fragment.DocumentId),
                Order = fragment.Order,
                Type = fragment.Type,
                Url = content["url"],
            };

        throw new NotImplementedException($"Sem suporte para fragmento Type={fragment.Type}");
    }

    public static string GetContent(this DocumentFragmentDto dto)
    {
        return dto switch
        {
            ArticleMarkdownFragmentDto markdown when dto.Type == DocumentFragmentType.Markdown
                => JsonSerializer.Serialize(new { markdown.Text }, jsonOptions),

            ArticleImageFragmentDto image when dto.Type == DocumentFragmentType.Image
                => JsonSerializer.Serialize(new { image.Url, image.Alt }, jsonOptions),

            ArticleEmbedFragmentDto embed when dto.Type == DocumentFragmentType.Embed
                => JsonSerializer.Serialize(new { embed.Url }, jsonOptions),

            _ => throw new NotImplementedException($"Dados inconsistentes em HashId='{dto.HashId}' Type={dto.Type}")
        };
    }

    public static string GetContent(this DocumentFragmentCreationDto dto)
    {
        return dto.ToFragmentDto().GetContent();
    }

    private static DocumentFragmentDto ToFragmentDto(this DocumentFragmentCreationDto dto)
    {
        if (dto.Type == DocumentFragmentType.Markdown)
            return new ArticleMarkdownFragmentDto
            {
                HashId = string.Empty,
                DocumentHashId = dto.DocumentHashId,
                Order = 0,
                Type = dto.Type,
                Text = string.Empty
            };

        if (dto.Type == DocumentFragmentType.Image)
            return new ArticleImageFragmentDto
            {
                HashId = string.Empty,
                DocumentHashId = dto.DocumentHashId,
                Order = 0,
                Type = dto.Type,
                Url = string.Empty,
                Alt = string.Empty
            };

        if (dto.Type == DocumentFragmentType.Embed)
            return new ArticleEmbedFragmentDto
            {
                HashId = string.Empty,
                DocumentHashId = dto.DocumentHashId,
                Order = 0,
                Type = dto.Type,
                Url = string.Empty
            };

        throw new NotImplementedException($"Sem suporte para fragmento Type={dto.Type}");
    }
}
