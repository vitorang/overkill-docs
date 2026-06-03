namespace OverkillDocs.Infrastructure.Interfaces;

internal interface ICacheInvalidator
{
    Task InvalidateAllScheduled();
    void MarkAsInvalid(string key);
}
