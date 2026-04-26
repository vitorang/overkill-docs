using OverkillDocs.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Account;

public sealed record PasswordChangeDto(
    [Required(ErrorMessage = "Senha obrigatória")]
    [DataType(DataType.Password)]
    string CurrentPassword,

    [Required(ErrorMessage = "Senha obrigatória")]
    [DataType(DataType.Password)]
    [Password]
    string NewPassword
);
