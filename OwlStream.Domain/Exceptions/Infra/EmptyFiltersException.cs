namespace OwlStream.Domain.Exceptions.Infra;

public class EmptyFiltersException : Exception
{
    public EmptyFiltersException() { }

    public EmptyFiltersException(string message) : base(message) { }

    public EmptyFiltersException(string message, Exception inner) : base(message, inner) { }
}
