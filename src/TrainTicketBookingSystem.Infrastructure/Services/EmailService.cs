using Microsoft.Extensions.Logging;
using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Application.Interfaces;

namespace TrainTicketBookingSystem.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // This is a mock implementation for demonstration
        // In a real application, you would integrate with an email service like SendGrid, AWS SES, etc.
        
        _logger.LogInformation("Sending email to {To} with subject: {Subject}", to, subject);
        _logger.LogDebug("Email body: {Body}", body);

        // Simulate email sending delay
        await Task.Delay(100, cancellationToken);

        _logger.LogInformation("Email sent successfully to {To}", to);
    }

    public async Task SendETicketAsync(BookingDto booking, CancellationToken cancellationToken = default)
    {
        try
        {
            var eTicketContent = GenerateETicket(booking);
            var subject = $"E-Ticket - {booking.BookingReference}";
            
            // In a real implementation, you would generate a PDF and attach it
            var userEmail = "user@example.com"; // This should come from booking user data
            await SendEmailAsync(userEmail, subject, eTicketContent, cancellationToken);

            _logger.LogInformation("E-ticket sent for booking {BookingReference}", booking.BookingReference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send e-ticket for booking {BookingReference}", booking.BookingReference);
        }
    }

    private static string GenerateETicket(BookingDto booking)
    {
        var ticketContent = $@"
            ================================
                   E-TICKET
            ================================
            
            Booking Reference: {booking.BookingReference}
            Booking Date: {booking.BookingDate:yyyy-MM-dd HH:mm}
            Status: {booking.Status}
            
            TRAIN DETAILS
            -------------
            Train: {booking.Schedule.Train.Name}
            Train Number: {booking.Schedule.Train.Number}
            
            JOURNEY DETAILS
            ---------------
            From: {booking.Schedule.Route.DepartureStation.Name}
            To: {booking.Schedule.Route.ArrivalStation.Name}
            Departure: {booking.Schedule.DepartureTime:yyyy-MM-dd HH:mm}
            Arrival: {booking.Schedule.ArrivalTime:yyyy-MM-dd HH:mm}
            
            PASSENGER DETAILS
            -----------------";

        foreach (var passenger in booking.Passengers)
        {
            ticketContent += $@"
            Name: {passenger.FullName}
            Age: {passenger.Age}
            ID: {passenger.IdentityNumber}";
        }

        ticketContent += $@"
            
            SEAT DETAILS
            ------------";

        foreach (var seat in booking.Seats)
        {
            ticketContent += $@"
            Seat: {seat.SeatNumber} ({seat.Class})";
        }

        ticketContent += $@"
            
            PAYMENT DETAILS
            ---------------
            Total Amount: ${booking.TotalAmount:F2}
            Payment Status: {booking.Payment?.Status.ToString() ?? "Pending"}
            
            ================================
            
            Please present this e-ticket at the station.
            Arrive at least 30 minutes before departure.
            
            Thank you for choosing our service!
            
            ================================
        ";

        return ticketContent;
    }
}