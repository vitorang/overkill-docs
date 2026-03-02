using OverkillDocs.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Account
{
    public record AuthRequestDto(
        [Required(ErrorMessage = "Usuário obrigatório")]
        [MinLength(3, ErrorMessage = "Mínimo de 3 caracteres")]
        [MinLength(15, ErrorMessage = "Máximo de 15 caracteres")]
        [Username]
        string Username,

        [Required(ErrorMessage = "Senha obrigatória")]
        [MinLength(3, ErrorMessage = "Mínimo de 3 caracteres")]
        string Password,
        
        [Required(ErrorMessage = "User-Agent é obrigatório")]
        string UserAgent
    );
}
