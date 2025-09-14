namespace TrainTicketBookingSystem.Application.DTOs;

public record StationDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public bool IsActive { get; init; }
}

public record RouteDto
{
    public int Id { get; init; }
    public StationDto DepartureStation { get; init; } = null!;
    public StationDto ArrivalStation { get; init; } = null!;
    public int DistanceKm { get; init; }
    public TimeSpan EstimatedDuration { get; init; }
    public bool IsActive { get; init; }
}