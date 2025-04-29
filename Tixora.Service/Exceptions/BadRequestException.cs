namespace Tixora.Service.Exceptions;

public class BadRequestException : Exception
{
    public Dictionary<string, string>? Errors { get; }

    public BadRequestException(string message = "Invalid request. Please check your input and try again.")
        : base(message)
    {
    }

    public BadRequestException(string message, Dictionary<string, string> errors)
        : base(message)
    {
        Errors = errors;
    }
}