using TrainTicketBookingSystem.Domain.Enums;

namespace TrainTicketBookingSystem.Domain.Entities;

public class Schedule : BaseEntity
{
    public int TrainId { get; set; }
    public int RouteId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal EconomyPrice { get; set; }
    public decimal BusinessPrice { get; set; }
    public decimal FirstClassPrice { get; set; }
    public int AvailableSeats { get; set; }

    // Navigation properties
    public virtual Train Train { get; set; } = null!;
    public virtual Route Route { get; set; } = null!;
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public decimal GetPriceForClass(SeatClass seatClass)
    {
        return seatClass switch
        {
            SeatClass.Economy => EconomyPrice,
            SeatClass.Business => BusinessPrice,
            SeatClass.FirstClass => FirstClassPrice,
            _ => EconomyPrice
        };
    }
}