using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OverkillDocs.Core.Attributes
{
    public partial class UsernameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string username)
            {
                if (username.Length is < 3 or > 15)
                    return new ValidationResult("Deve ter de 3 a 15 caracteres");

                if (!UsernameRegex().IsMatch(username))
                    return new ValidationResult("Use apenas letras minúsculas e números");

                return ValidationResult.Success;
            }

            throw new InvalidOperationException($"Tipo não suportado em {nameof(UsernameAttribute)}");            
        }

        [GeneratedRegex(@"^[a-z0-9]+$")]
        private static partial Regex UsernameRegex();
    }
}
