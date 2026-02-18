using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.DTOs.Account
{
    public record AuthResponseDto(
        [Required]
        string Token
    );
}
