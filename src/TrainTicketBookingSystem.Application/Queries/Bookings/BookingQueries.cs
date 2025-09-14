using AutoMapper;
using MediatR;
using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Application.Interfaces;
using TrainTicketBookingSystem.Domain.Interfaces;

namespace TrainTicketBookingSystem.Application.Queries.Bookings;

public record GetUserBookingsQuery(int UserId) : IRequest<IEnumerable<BookingDto>>;

public class GetUserBookingsQueryHandler : IRequestHandler<GetUserBookingsQuery, IEnumerable<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserBookingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookingDto>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _unitOfWork.Bookings.GetByUserIdAsync(request.UserId, cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }
}

public record GetBookingByIdQuery(int BookingId) : IRequest<BookingDto>;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBookingByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BookingDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId, cancellationToken);
        return _mapper.Map<BookingDto>(booking);
    }
}

public record GetBookingByReferenceQuery(string Reference) : IRequest<BookingDto>;

public class GetBookingByReferenceQueryHandler : IRequestHandler<GetBookingByReferenceQuery, BookingDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBookingByReferenceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BookingDto> Handle(GetBookingByReferenceQuery request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Bookings.GetByReferenceAsync(request.Reference, cancellationToken);
        return _mapper.Map<BookingDto>(booking);
    }
}