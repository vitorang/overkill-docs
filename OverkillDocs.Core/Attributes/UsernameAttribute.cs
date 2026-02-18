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
                if (username.Length < 3 || username.Length > 15)
                    return new ValidationResult("O nome de usuário deve ter entre 3 e 15 caracteres.");

                if (!UsernameRegex().IsMatch(username))
                    return new ValidationResult("O nome de usuário deve conter apenas letras minúsculas e números.");
            }

            return ValidationResult.Success;
        }

        [GeneratedRegex(@"^[a-z0-9]+$")]
        private static partial Regex UsernameRegex();
    }
}
