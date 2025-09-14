using TrainTicketBookingSystem.Domain.Enums;

namespace TrainTicketBookingSystem.Application.DTOs;

public record BookingDto
{
    public int Id { get; init; }
    public string BookingReference { get; init; } = string.Empty;
    public int NumberOfPassengers { get; init; }
    public decimal TotalAmount { get; init; }
    public BookingStatus Status { get; init; }
    public DateTime BookingDate { get; init; }
    public ScheduleDto Schedule { get; init; } = null!;
    public List<PassengerDto> Passengers { get; init; } = new();
    public List<SeatDto> Seats { get; init; } = new();
    public PaymentDto? Payment { get; init; }
}

public record CreateBookingDto
{
    public int ScheduleId { get; init; }
    public List<CreatePassengerDto> Passengers { get; init; } = new();
    public List<int> SeatIds { get; init; } = new();
    public SeatClass SeatClass { get; init; }
}

public record PassengerDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string IdentityNumber { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public int Age { get; init; }
}

public record CreatePassengerDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string IdentityNumber { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}