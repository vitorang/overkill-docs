using Bogus;
using OverkillDocs.Core.DTOs.Document;
using OverkillDocs.Core.Enums;

namespace OverkillDocs.Tests.Common.Fakers.DTOs.Document;

public sealed class DocumentSummaryDtoFaker : Faker<DocumentSummaryDto>
{
    public DocumentSummaryDtoFaker()
    {
        CustomInstantiator(f => new DocumentSummaryDto(
            Title: f.Company.CompanyName(),
            HashId: string.Empty,
            Type: DocumentType.Article
        ));
    }
}
