using System.ComponentModel.DataAnnotations;

namespace OverkillDocs.Core.Attributes
{
    public partial class PasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string password)
            {
                if (password.Length is < 3 or > 15)
                    return new ValidationResult("Deve ter de 3 a 15 caracteres");

                return ValidationResult.Success;
            }

            throw new InvalidOperationException($"Tipo não suportado em {nameof(PasswordAttribute)}");
        }
    }
}
