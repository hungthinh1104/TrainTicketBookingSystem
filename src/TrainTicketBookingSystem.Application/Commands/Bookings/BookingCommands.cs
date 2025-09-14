using AutoMapper;
using MediatR;
using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Application.Interfaces;
using TrainTicketBookingSystem.Domain.Entities;
using TrainTicketBookingSystem.Domain.Enums;
using TrainTicketBookingSystem.Domain.Exceptions;
using TrainTicketBookingSystem.Domain.Interfaces;

namespace TrainTicketBookingSystem.Application.Commands.Bookings;

public record CreateBookingCommand(CreateBookingDto BookingDto) : IRequest<BookingDto>;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBookingService _bookingService;
    private readonly IPricingStrategy _pricingStrategy;

    public CreateBookingCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IBookingService bookingService,
        IPricingStrategy pricingStrategy)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _bookingService = bookingService;
        _pricingStrategy = pricingStrategy;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(request.BookingDto.ScheduleId, cancellationToken);
        if (schedule == null)
        {
            throw new EntityNotFoundException(nameof(Schedule), request.BookingDto.ScheduleId);
        }

        if (schedule.AvailableSeats < request.BookingDto.Passengers.Count)
        {
            throw new BookingNotAvailableException("Not enough available seats for this schedule.");
        }

        // Check seat availability
        var availableSeats = await _unitOfWork.Seats.GetAvailableSeatsAsync(request.BookingDto.ScheduleId, cancellationToken);
        var requestedSeats = request.BookingDto.SeatIds;
        
        if (!requestedSeats.All(seatId => availableSeats.Any(s => s.Id == seatId)))
        {
            throw new BookingNotAvailableException("One or more requested seats are not available.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Create booking
            var booking = _mapper.Map<Booking>(request.BookingDto);
            booking.UserId = _currentUserService.UserId;
            booking.BookingReference = await _bookingService.GenerateBookingReferenceAsync(cancellationToken);
            
            // Calculate total amount
            var basePrice = schedule.GetPriceForClass(request.BookingDto.SeatClass);
            var totalAmount = _pricingStrategy.CalculatePrice(basePrice, request.BookingDto.SeatClass, DateTime.UtcNow, schedule.DepartureTime);
            booking.TotalAmount = totalAmount * request.BookingDto.Passengers.Count;
            booking.NumberOfPassengers = request.BookingDto.Passengers.Count;

            var createdBooking = await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create passengers
            foreach (var passengerDto in request.BookingDto.Passengers)
            {
                var passenger = _mapper.Map<Passenger>(passengerDto);
                passenger.BookingId = createdBooking.Id;
                await _unitOfWork.Passengers.AddAsync(passenger, cancellationToken);
            }

            // Reserve seats
            var passengers = createdBooking.Passengers.ToList();
            for (int i = 0; i < requestedSeats.Count && i < passengers.Count; i++)
            {
                var bookingSeat = new BookingSeat
                {
                    BookingId = createdBooking.Id,
                    SeatId = requestedSeats[i],
                    PassengerId = passengers[i].Id
                };
                await _unitOfWork.BookingSeats.AddAsync(bookingSeat, cancellationToken);
            }

            // Update available seats
            schedule.AvailableSeats -= request.BookingDto.Passengers.Count;
            await _unitOfWork.Schedules.UpdateAsync(schedule, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Fetch complete booking with related data
            var completeBooking = await _unitOfWork.Bookings.GetByIdAsync(createdBooking.Id, cancellationToken);
            return _mapper.Map<BookingDto>(completeBooking);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

public record ConfirmBookingCommand(int BookingId) : IRequest<BookingDto>;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, BookingDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public ConfirmBookingCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<BookingDto> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            throw new EntityNotFoundException(nameof(Booking), request.BookingId);
        }

        if (booking.Status != BookingStatus.Pending)
        {
            throw new InvalidBookingStateException("Only pending bookings can be confirmed.");
        }

        booking.Confirm();
        await _unitOfWork.Bookings.UpdateAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var bookingDto = _mapper.Map<BookingDto>(booking);
        await _notificationService.SendBookingConfirmationAsync(bookingDto, cancellationToken);

        return bookingDto;
    }
}

public record CancelBookingCommand(int BookingId) : IRequest<BookingDto>;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, BookingDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public CancelBookingCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<BookingDto> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            throw new EntityNotFoundException(nameof(Booking), request.BookingId);
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new InvalidBookingStateException("Booking is already cancelled.");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            throw new InvalidBookingStateException("Completed bookings cannot be cancelled.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            booking.Cancel();
            await _unitOfWork.Bookings.UpdateAsync(booking, cancellationToken);

            // Restore available seats
            var schedule = await _unitOfWork.Schedules.GetByIdAsync(booking.ScheduleId, cancellationToken);
            if (schedule != null)
            {
                schedule.AvailableSeats += booking.NumberOfPassengers;
                await _unitOfWork.Schedules.UpdateAsync(schedule, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var bookingDto = _mapper.Map<BookingDto>(booking);
            await _notificationService.SendBookingCancellationAsync(bookingDto, cancellationToken);

            return bookingDto;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}