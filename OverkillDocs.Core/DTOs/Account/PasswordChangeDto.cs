using OverkillDocs.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Account
{
    public record PasswordChangeDto(
        [Required(ErrorMessage = "Senha obrigatória")]
        string CurrentPassword,

        [Required(ErrorMessage = "Senha obrigatória")]
        [Password]
        string NewPassword
    );
}
