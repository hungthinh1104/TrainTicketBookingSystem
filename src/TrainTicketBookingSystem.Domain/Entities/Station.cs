namespace TrainTicketBookingSystem.Domain.Entities;

public class Station : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Route> DepartureRoutes { get; set; } = new List<Route>();
    public virtual ICollection<Route> ArrivalRoutes { get; set; } = new List<Route>();
}