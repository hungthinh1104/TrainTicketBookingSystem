namespace TrainTicketBookingSystem.Domain.Entities;

public class BookingSeat : BaseEntity
{
    public int BookingId { get; set; }
    public int SeatId { get; set; }
    public int PassengerId { get; set; }

    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
    public virtual Seat Seat { get; set; } = null!;
    public virtual Passenger Passenger { get; set; } = null!;
}