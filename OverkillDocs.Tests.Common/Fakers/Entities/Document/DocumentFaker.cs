using Bogus;
using OverkillDocs.Core.Enums;
using DocumentEntity = OverkillDocs.Core.Entities.Document.Document;

namespace OverkillDocs.Tests.Common.Fakers.Entities.Document;

public class DocumentFaker : Faker<DocumentEntity>
{
    public DocumentFaker()
    {
        CustomInstantiator(f => new DocumentEntity
        {
            Title = f.Company.CompanyName(),
            Type = DocumentType.Article
        });
    }
}
