namespace TrainTicketBookingSystem.Domain.Events;

public class BookingCancelledEvent : IDomainEvent
{
    public int BookingId { get; }
    public int UserId { get; }
    public string BookingReference { get; }
    public DateTime OccurredOn { get; }

    public BookingCancelledEvent(int bookingId, int userId, string bookingReference)
    {
        BookingId = bookingId;
        UserId = userId;
        BookingReference = bookingReference;
        OccurredOn = DateTime.UtcNow;
    }
}