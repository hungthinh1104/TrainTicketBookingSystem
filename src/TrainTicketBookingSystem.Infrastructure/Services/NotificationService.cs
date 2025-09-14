using Microsoft.Extensions.Logging;
using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Application.Interfaces;

namespace TrainTicketBookingSystem.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IEmailService emailService, ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SendBookingConfirmationAsync(BookingDto booking, CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = $"Booking Confirmation - {booking.BookingReference}";
            var body = GenerateBookingConfirmationEmail(booking);
            
            // Send to booking user
            var userEmail = booking.Schedule.Train.Name; // This should be from user data
            await _emailService.SendEmailAsync(userEmail, subject, body, cancellationToken);

            // Send e-ticket
            await _emailService.SendETicketAsync(booking, cancellationToken);

            _logger.LogInformation("Booking confirmation sent for booking {BookingReference}", booking.BookingReference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send booking confirmation for booking {BookingReference}", booking.BookingReference);
        }
    }

    public async Task SendBookingCancellationAsync(BookingDto booking, CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = $"Booking Cancellation - {booking.BookingReference}";
            var body = GenerateBookingCancellationEmail(booking);
            
            var userEmail = booking.Schedule.Train.Name; // This should be from user data
            await _emailService.SendEmailAsync(userEmail, subject, body, cancellationToken);

            _logger.LogInformation("Booking cancellation sent for booking {BookingReference}", booking.BookingReference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send booking cancellation for booking {BookingReference}", booking.BookingReference);
        }
    }

    public async Task SendPaymentConfirmationAsync(PaymentDto payment, CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = $"Payment Confirmation - {payment.TransactionId}";
            var body = GeneratePaymentConfirmationEmail(payment);
            
            // This should get user email from payment data
            var userEmail = "user@example.com"; 
            await _emailService.SendEmailAsync(userEmail, subject, body, cancellationToken);

            _logger.LogInformation("Payment confirmation sent for transaction {TransactionId}", payment.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment confirmation for transaction {TransactionId}", payment.TransactionId);
        }
    }

    private static string GenerateBookingConfirmationEmail(BookingDto booking)
    {
        return $@"
            Dear Customer,

            Your train booking has been confirmed!

            Booking Reference: {booking.BookingReference}
            Train: {booking.Schedule.Train.Name} ({booking.Schedule.Train.Number})
            From: {booking.Schedule.Route.DepartureStation.Name}
            To: {booking.Schedule.Route.ArrivalStation.Name}
            Departure: {booking.Schedule.DepartureTime:yyyy-MM-dd HH:mm}
            Arrival: {booking.Schedule.ArrivalTime:yyyy-MM-dd HH:mm}
            Passengers: {booking.NumberOfPassengers}
            Total Amount: ${booking.TotalAmount:F2}

            Please arrive at the station at least 30 minutes before departure.

            Thank you for choosing our service!
        ";
    }

    private static string GenerateBookingCancellationEmail(BookingDto booking)
    {
        return $@"
            Dear Customer,

            Your train booking has been cancelled.

            Booking Reference: {booking.BookingReference}
            Train: {booking.Schedule.Train.Name} ({booking.Schedule.Train.Number})
            From: {booking.Schedule.Route.DepartureStation.Name}
            To: {booking.Schedule.Route.ArrivalStation.Name}
            Departure: {booking.Schedule.DepartureTime:yyyy-MM-dd HH:mm}

            If you paid for this booking, a refund will be processed within 3-5 business days.

            Thank you for choosing our service!
        ";
    }

    private static string GeneratePaymentConfirmationEmail(PaymentDto payment)
    {
        return $@"
            Dear Customer,

            Your payment has been processed successfully!

            Transaction ID: {payment.TransactionId}
            Amount: ${payment.Amount:F2}
            Payment Method: {payment.Method}
            Status: {payment.Status}
            Processed At: {payment.ProcessedAt:yyyy-MM-dd HH:mm}

            Thank you for your payment!
        ";
    }
}