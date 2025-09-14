using AutoMapper;
using MediatR;
using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Application.Interfaces;
using TrainTicketBookingSystem.Domain.Interfaces;

namespace TrainTicketBookingSystem.Application.Queries.Schedules;

public record SearchSchedulesQuery(SearchScheduleDto SearchDto) : IRequest<IEnumerable<ScheduleDto>>;

public class SearchSchedulesQueryHandler : IRequestHandler<SearchSchedulesQuery, IEnumerable<ScheduleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public SearchSchedulesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<ScheduleDto>> Handle(SearchSchedulesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"schedules_{request.SearchDto.DepartureStationId}_{request.SearchDto.ArrivalStationId}_{request.SearchDto.DepartureDate:yyyy-MM-dd}";
        
        var cachedSchedules = await _cacheService.GetAsync<IEnumerable<ScheduleDto>>(cacheKey, cancellationToken);
        if (cachedSchedules != null)
        {
            return cachedSchedules;
        }

        var schedules = await _unitOfWork.Schedules.SearchSchedulesAsync(
            request.SearchDto.DepartureStationId,
            request.SearchDto.ArrivalStationId,
            request.SearchDto.DepartureDate,
            cancellationToken);

        var scheduleDtos = _mapper.Map<IEnumerable<ScheduleDto>>(schedules);

        // Cache for 10 minutes
        await _cacheService.SetAsync(cacheKey, scheduleDtos, TimeSpan.FromMinutes(10), cancellationToken);

        return scheduleDtos;
    }
}

public record GetScheduleByIdQuery(int ScheduleId) : IRequest<ScheduleDto>;

public class GetScheduleByIdQueryHandler : IRequestHandler<GetScheduleByIdQuery, ScheduleDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetScheduleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ScheduleDto> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(request.ScheduleId, cancellationToken);
        return _mapper.Map<ScheduleDto>(schedule);
    }
}

public record GetAvailableSeatsQuery(int ScheduleId) : IRequest<IEnumerable<SeatDto>>;

public class GetAvailableSeatsQueryHandler : IRequestHandler<GetAvailableSeatsQuery, IEnumerable<SeatDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAvailableSeatsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SeatDto>> Handle(GetAvailableSeatsQuery request, CancellationToken cancellationToken)
    {
        var seats = await _unitOfWork.Seats.GetAvailableSeatsAsync(request.ScheduleId, cancellationToken);
        return _mapper.Map<IEnumerable<SeatDto>>(seats);
    }
}