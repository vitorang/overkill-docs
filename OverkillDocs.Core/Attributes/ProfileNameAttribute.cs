using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.Attributes;

public class ProfileNameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string strValue)
        {
            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult("Nome é obrigatório");

            if (strValue.Length > 15)
                return new ValidationResult("Deve ter até 15 caracteres");

            if (strValue.Trim().Length != strValue.Length)
                return new ValidationResult("Nome inválido");

            return ValidationResult.Success;
        }

        throw new InvalidOperationException($"Tipo não suportado em validador");
    }
}
