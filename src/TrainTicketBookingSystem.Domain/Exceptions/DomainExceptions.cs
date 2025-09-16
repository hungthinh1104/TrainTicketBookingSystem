namespace TrainTicketBookingSystem.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id) 
        : base($"{entityName} with id '{id}' was not found.") { }
}

public class BookingNotAvailableException : DomainException
{
    public BookingNotAvailableException(string message) : base(message) { }
}

public class PaymentProcessingException : DomainException
{
    public PaymentProcessingException(string message) : base(message) { }
}

public class InvalidBookingStateException : DomainException
{
    public InvalidBookingStateException(string message) : base(message) { }
}