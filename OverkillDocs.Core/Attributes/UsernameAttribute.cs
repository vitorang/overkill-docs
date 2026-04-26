using OverkillDocs.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OverkillDocs.Core.Attributes;

public partial class UsernameAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string strValue)
        {
            if (strValue.Length is < 3 or > 15)
                return new ValidationResult("Deve ter de 3 a 15 caracteres");

            if (!UsernameRegex().IsMatch(strValue))
                return new ValidationResult("Use apenas letras minúsculas e números");

            if (strValue.StartsWith(AccountConstants.AnonymizedPrefix))
                return new ValidationResult("Nome inválido.");

            return ValidationResult.Success;
        }

        throw new InvalidOperationException($"Tipo não suportado em validador");
    }

    [GeneratedRegex(@"^[a-z0-9]+$")]
    private static partial Regex UsernameRegex();
}
