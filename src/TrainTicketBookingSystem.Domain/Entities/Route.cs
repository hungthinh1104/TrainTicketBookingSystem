namespace TrainTicketBookingSystem.Domain.Entities;

public class Route : BaseEntity
{
    public int DepartureStationId { get; set; }
    public int ArrivalStationId { get; set; }
    public int DistanceKm { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Station DepartureStation { get; set; } = null!;
    public virtual Station ArrivalStation { get; set; } = null!;
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}