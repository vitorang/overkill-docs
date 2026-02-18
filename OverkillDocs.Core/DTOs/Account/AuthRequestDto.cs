using OverkillDocs.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Account
{
    public record AuthRequestDto(
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [Username]
        string Username,

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(3, ErrorMessage = "A senha deve no mínimo 3 caracteres.")]
        string Password,
        
        [Required(ErrorMessage = "O agente é obrigatório")]
        string UserAgent
    );
}
