namespace OverkillDocs.Core.Interfaces.Services
{
    public interface IPasswordService
    {
        string CalculeHash(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
