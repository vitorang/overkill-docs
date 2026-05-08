using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.Attributes;

public sealed class DocumentNameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string strValue)
        {
            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult("Nome é obrigatório");

            if (strValue.Trim().Length != strValue.Length)
                return new ValidationResult("Remova espaços do início e fim do nome");

            if (strValue.Length is < 3 or > 100)
                return new ValidationResult("Deve ter de 3 a 100 caracteres");

            return ValidationResult.Success;
        }

        throw new InvalidOperationException($"Tipo não suportado em validador");
    }
}
