using OverkillDocs.Core.Enums;

namespace OverkillDocs.Core.DTOs.Document.Fragment;

public abstract record DocumentFragmentDto
{
    public required string HashId { get; init; }
    public required string DocumentHashId { get; init; }
    public DocumentFragmentType Type { get; init; }
    public double Order { get; init; }
}
