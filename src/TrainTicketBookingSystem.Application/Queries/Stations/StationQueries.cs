using AutoMapper;
using MediatR;
using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Application.Interfaces;
using TrainTicketBookingSystem.Domain.Interfaces;

namespace TrainTicketBookingSystem.Application.Queries.Stations;

public record GetAllStationsQuery : IRequest<IEnumerable<StationDto>>;

public class GetAllStationsQueryHandler : IRequestHandler<GetAllStationsQuery, IEnumerable<StationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetAllStationsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<StationDto>> Handle(GetAllStationsQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "all_stations";
        
        var cachedStations = await _cacheService.GetAsync<IEnumerable<StationDto>>(cacheKey, cancellationToken);
        if (cachedStations != null)
        {
            return cachedStations;
        }

        var stations = await _unitOfWork.Stations.GetAllAsync(cancellationToken);
        var stationDtos = _mapper.Map<IEnumerable<StationDto>>(stations);

        // Cache for 1 hour since stations don't change frequently
        await _cacheService.SetAsync(cacheKey, stationDtos, TimeSpan.FromHours(1), cancellationToken);

        return stationDtos;
    }
}

public record GetStationsByCityQuery(string City) : IRequest<IEnumerable<StationDto>>;

public class GetStationsByCityQueryHandler : IRequestHandler<GetStationsByCityQuery, IEnumerable<StationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStationsByCityQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StationDto>> Handle(GetStationsByCityQuery request, CancellationToken cancellationToken)
    {
        var stations = await _unitOfWork.Stations.GetByCityAsync(request.City, cancellationToken);
        return _mapper.Map<IEnumerable<StationDto>>(stations);
    }
}