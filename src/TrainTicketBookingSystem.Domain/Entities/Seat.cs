using TrainTicketBookingSystem.Domain.Enums;

namespace TrainTicketBookingSystem.Domain.Entities;

public class Seat : BaseEntity
{
    public int TrainId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public SeatClass Class { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Navigation properties
    public virtual Train Train { get; set; } = null!;
    public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
}