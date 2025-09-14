namespace TrainTicketBookingSystem.Domain.Events;

public class BookingConfirmedEvent : IDomainEvent
{
    public int BookingId { get; }
    public int UserId { get; }
    public string BookingReference { get; }
    public DateTime OccurredOn { get; }

    public BookingConfirmedEvent(int bookingId, int userId, string bookingReference)
    {
        BookingId = bookingId;
        UserId = userId;
        BookingReference = bookingReference;
        OccurredOn = DateTime.UtcNow;
    }
}