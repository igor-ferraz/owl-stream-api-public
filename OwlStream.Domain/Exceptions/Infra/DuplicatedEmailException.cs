namespace OwlStream.Domain.Exceptions.Infra;

public class DuplicatedEmailException : Exception
{
    public DuplicatedEmailException() { }

    public DuplicatedEmailException(string message) : base(message) { }

    public DuplicatedEmailException(string message, Exception inner) : base(message, inner) { }
}