using OverkillDocs.Core.Interfaces.Services;
using BC = BCrypt.Net.BCrypt;

namespace OverkillDocs.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private const int WorkFactor = 4;

    public string CalculeHash(string password)
    {
        return BC.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BC.Verify(password, passwordHash);
    }
}
