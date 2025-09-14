using TrainTicketBookingSystem.Domain.Enums;
using TrainTicketBookingSystem.Domain.Events;

namespace TrainTicketBookingSystem.Domain.Entities;

public class Booking : BaseEntity
{
    public int UserId { get; set; }
    public int ScheduleId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int NumberOfPassengers { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime BookingDate { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Schedule Schedule { get; set; } = null!;
    public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    public virtual ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
    public virtual Payment? Payment { get; set; }

    public void Confirm()
    {
        Status = BookingStatus.Confirmed;
        AddDomainEvent(new BookingConfirmedEvent(Id, UserId, BookingReference));
    }

    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
        AddDomainEvent(new BookingCancelledEvent(Id, UserId, BookingReference));
    }
}