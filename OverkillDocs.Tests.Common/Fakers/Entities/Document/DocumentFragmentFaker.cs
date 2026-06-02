using Bogus;
using OverkillDocs.Core.Entities.Document;
using OverkillDocs.Core.Enums;
using System.Text.Json;
using DocumentEntity = OverkillDocs.Core.Entities.Document.Document;

namespace OverkillDocs.Tests.Common.Fakers.Entities.Document;

public class DocumentFragmentFaker : Faker<DocumentFragment>
{
    public DocumentFragmentFaker(DocumentEntity document)
    {
        CustomInstantiator(f => new DocumentFragment
        {
            Content = JsonSerializer.Serialize(new { Text = f.Lorem.Paragraph() }),
            Document = document,
            Type = DocumentFragmentType.Markdown,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }
}
