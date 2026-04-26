namespace OverkillDocs.Core.Exceptions;

public abstract class CoreException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public class ConflictException(string message) : CoreException(message, 409);
public class ForbiddenException(string message) : CoreException(message, 403);
public class NotFoundException(string message) : CoreException(message, 404);
public class UnauthorizedException(string message) : CoreException(message, 401);
