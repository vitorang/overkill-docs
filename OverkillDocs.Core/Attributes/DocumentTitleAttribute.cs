using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.Attributes;

public sealed class DocumentTitleAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string strValue)
        {
            if (string.IsNullOrWhiteSpace(strValue))
                return new ValidationResult("Nome é obrigatório");

            if (strValue.Trim().Length < strValue.Length)
                return new ValidationResult("Remova espaços do início e fim do nome");


            if (strValue.Length > 100)
                return new ValidationResult("Deve ter até 100 caracteres");

            return ValidationResult.Success;
        }

        throw new InvalidOperationException($"Tipo não suportado em validador");
    }
}
