using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Account;

public record AccountDeletionDto(
    [Required]
    [DataType(DataType.Password)]
    string Password
);
