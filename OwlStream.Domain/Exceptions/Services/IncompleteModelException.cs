namespace OwlStream.Domain.Exceptions.Services;

public class IncompleteModelException : Exception
{
    public IncompleteModelException() { }

    public IncompleteModelException(string message) : base(message) { }

    public IncompleteModelException(string message, Exception inner) : base(message, inner) { }
}
