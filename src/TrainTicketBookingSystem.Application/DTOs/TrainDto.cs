using TrainTicketBookingSystem.Domain.Enums;

namespace TrainTicketBookingSystem.Application.DTOs;

public record TrainDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Number { get; init; } = string.Empty;
    public int TotalSeats { get; init; }
    public int EconomySeats { get; init; }
    public int BusinessSeats { get; init; }
    public int FirstClassSeats { get; init; }
    public TrainStatus Status { get; init; }
}

public record ScheduleDto
{
    public int Id { get; init; }
    public TrainDto Train { get; init; } = null!;
    public RouteDto Route { get; init; } = null!;
    public DateTime DepartureTime { get; init; }
    public DateTime ArrivalTime { get; init; }
    public decimal EconomyPrice { get; init; }
    public decimal BusinessPrice { get; init; }
    public decimal FirstClassPrice { get; init; }
    public int AvailableSeats { get; init; }
}

public record SearchScheduleDto
{
    public int DepartureStationId { get; init; }
    public int ArrivalStationId { get; init; }
    public DateTime DepartureDate { get; init; }
    public SeatClass? PreferredClass { get; init; }
}

public record SeatDto
{
    public int Id { get; init; }
    public string SeatNumber { get; init; } = string.Empty;
    public SeatClass Class { get; init; }
    public bool IsAvailable { get; init; }
}