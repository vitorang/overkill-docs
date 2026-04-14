using OverkillDocs.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Account
{
    public record AuthRequestDto(
        [Required(ErrorMessage = "Usuário obrigatório")]
        [Username]
        string Username,

        [Required(ErrorMessage = "Senha obrigatória")]
        [Password]
        string Password,
        
        [Required(ErrorMessage = "User-Agent é obrigatório")]
        string UserAgent
    );
}
