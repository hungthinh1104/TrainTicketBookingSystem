using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Domain.Enums;

namespace TrainTicketBookingSystem.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto, CancellationToken cancellationToken = default);
    Task<BookingDto> ConfirmBookingAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<BookingDto> CancelBookingAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<string> GenerateBookingReferenceAsync(CancellationToken cancellationToken = default);
}

public interface IPaymentService
{
    Task<PaymentDto> ProcessPaymentAsync(CreatePaymentDto createPaymentDto, CancellationToken cancellationToken = default);
    Task<PaymentDto> ConfirmPaymentAsync(ProcessPaymentDto processPaymentDto, CancellationToken cancellationToken = default);
}

public interface IPricingStrategy
{
    decimal CalculatePrice(decimal basePrice, SeatClass seatClass, DateTime bookingDate, DateTime departureDate);
}

public interface INotificationService
{
    Task SendBookingConfirmationAsync(BookingDto booking, CancellationToken cancellationToken = default);
    Task SendBookingCancellationAsync(BookingDto booking, CancellationToken cancellationToken = default);
    Task SendPaymentConfirmationAsync(PaymentDto payment, CancellationToken cancellationToken = default);
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendETicketAsync(BookingDto booking, CancellationToken cancellationToken = default);
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}