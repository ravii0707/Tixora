// Tixora.Service/Exceptions/NotFoundException.cs (UPDATED)
namespace Tixora.Service.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string resourceName, int id)
        : base($"{resourceName} with ID {id} was not found. Please verify and try again.")
    {
    }

    public NotFoundException(string message = "The requested resource was not found.")
        : base(message)
    {
    }
}