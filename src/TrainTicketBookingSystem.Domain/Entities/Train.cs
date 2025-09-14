using TrainTicketBookingSystem.Domain.Enums;

namespace TrainTicketBookingSystem.Domain.Entities;

public class Train : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public int EconomySeats { get; set; }
    public int BusinessSeats { get; set; }
    public int FirstClassSeats { get; set; }
    public TrainStatus Status { get; set; } = TrainStatus.Active;

    // Navigation properties
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}