using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Application.Interfaces;
using TrainTicketBookingSystem.Domain.Interfaces;

namespace TrainTicketBookingSystem.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Booking creation is handled by CreateBookingCommandHandler");
    }

    public async Task<BookingDto> ConfirmBookingAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Booking confirmation is handled by ConfirmBookingCommandHandler");
    }

    public async Task<BookingDto> CancelBookingAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Booking cancellation is handled by CancelBookingCommandHandler");
    }

    public async Task<string> GenerateBookingReferenceAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Bookings.GenerateBookingReferenceAsync(cancellationToken);
    }
}