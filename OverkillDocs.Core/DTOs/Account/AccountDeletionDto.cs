using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Account;

public sealed record AccountDeletionDto(
    [Required]
    [DataType(DataType.Password)]
    string Password
);
