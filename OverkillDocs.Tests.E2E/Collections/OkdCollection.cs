using OverkillDocs.Tests.E2E.Fixtures;

namespace OverkillDocs.Tests.E2E.Collections;


[CollectionDefinition(Name)]
public class OkdCollection : ICollectionFixture<PlaywrightFixture>
{
    public const string Name = "OverkillDocs E2E Collection";
}
