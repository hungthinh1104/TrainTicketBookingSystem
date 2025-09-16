using TrainTicketBookingSystem.Domain.Enums;

namespace TrainTicketBookingSystem.Application.DTOs;

public record PaymentDto
{
    public int Id { get; init; }
    public decimal Amount { get; init; }
    public PaymentMethod Method { get; init; }
    public PaymentStatus Status { get; init; }
    public string TransactionId { get; init; } = string.Empty;
    public DateTime? ProcessedAt { get; init; }
}

public record CreatePaymentDto
{
    public int BookingId { get; init; }
    public PaymentMethod Method { get; init; }
    public decimal Amount { get; init; }
}

public record ProcessPaymentDto
{
    public int PaymentId { get; init; }
    public string TransactionId { get; init; } = string.Empty;
    public string? GatewayResponse { get; init; }
}