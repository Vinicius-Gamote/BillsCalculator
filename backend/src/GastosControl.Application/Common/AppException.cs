namespace GastosControl.Application.Common;

public abstract class AppException : Exception
{
    protected AppException(string message)
        : base(message)
    {
    }
}

public sealed class ValidationAppException : AppException
{
    public ValidationAppException(string message)
        : base(message)
    {
    }
}

public sealed class ConflictAppException : AppException
{
    public ConflictAppException(string message)
        : base(message)
    {
    }
}

public sealed class NotFoundAppException : AppException
{
    public NotFoundAppException(string message)
        : base(message)
    {
    }
}

public sealed class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message)
        : base(message)
    {
    }
}
