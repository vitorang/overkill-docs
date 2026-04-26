namespace OverkillDocs.Core.Exceptions;

public abstract class CoreException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public sealed class ConflictException(string message) : CoreException(message, 409);
public sealed class ForbiddenException(string message) : CoreException(message, 403);
public sealed class NotFoundException(string message) : CoreException(message, 404);
public sealed class UnauthorizedException(string message) : CoreException(message, 401);
