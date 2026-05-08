using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.Attributes;

public sealed class SearchTextAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string strValue)
        {
            if (strValue.Length > 200)
                return new ValidationResult("Deve ter até 200 caracteres");

            if (strValue.Trim().Length != strValue.Length)
                return new ValidationResult("Não pode ter espaços no início ou fim");

            if (strValue.Any(char.IsUpper))
                return new ValidationResult("Só pode ter letras minúsculas");

            return ValidationResult.Success;
        }

        throw new InvalidOperationException($"Tipo não suportado em validador");
    }
}
