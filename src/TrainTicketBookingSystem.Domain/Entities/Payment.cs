using TrainTicketBookingSystem.Domain.Enums;

namespace TrainTicketBookingSystem.Domain.Entities;

public class Payment : BaseEntity
{
    public int UserId { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string TransactionId { get; set; } = string.Empty;
    public string? GatewayResponse { get; set; }
    public DateTime? ProcessedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Booking Booking { get; set; } = null!;
}