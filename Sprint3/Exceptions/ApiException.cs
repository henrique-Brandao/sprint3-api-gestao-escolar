namespace Sprint3.Exceptions;

public abstract class ApiException : Exception
{
    protected ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}

public class NotFoundException : ApiException
{
    public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound) { }
}

public class BadRequestException : ApiException
{
    public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest) { }
}

public class ForbiddenException : ApiException
{
    public ForbiddenException(string message) : base(message, StatusCodes.Status403Forbidden) { }
}
