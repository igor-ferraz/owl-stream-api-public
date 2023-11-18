namespace OwlStream.Domain.Exceptions.Services;

public class InvalidFileMimeTypeException : Exception
{
    public InvalidFileMimeTypeException() { }

    public InvalidFileMimeTypeException(string message) : base(message) { }

    public InvalidFileMimeTypeException(string message, Exception inner) : base(message, inner) { }
}
