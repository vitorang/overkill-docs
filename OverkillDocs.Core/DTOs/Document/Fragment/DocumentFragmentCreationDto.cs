using OverkillDocs.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Document.Fragment;

public record DocumentFragmentCreationDto(
    [Required]
    string DocumentHashId,
    DocumentFragmentType Type,
    string? InsertAfterHashId
);
