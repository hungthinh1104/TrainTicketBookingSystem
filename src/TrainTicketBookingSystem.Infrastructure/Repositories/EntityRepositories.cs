using Microsoft.EntityFrameworkCore;
using TrainTicketBookingSystem.Domain.Entities;
using TrainTicketBookingSystem.Domain.Interfaces;
using TrainTicketBookingSystem.Infrastructure.Data;

namespace TrainTicketBookingSystem.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}

public class StationRepository : Repository<Station>, IStationRepository
{
    public StationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Station?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<Station>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.City.Contains(city) && s.IsActive)
            .ToListAsync(cancellationToken);
    }
}

public class TrainRepository : Repository<Train>, ITrainRepository
{
    public TrainRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Train?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Number == number, cancellationToken);
    }

    public async Task<IEnumerable<Train>> GetActiveTrainsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Status == Domain.Enums.TrainStatus.Active)
            .ToListAsync(cancellationToken);
    }
}

public class RouteRepository : Repository<Route>, IRouteRepository
{
    public RouteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Route>> GetByStationsAsync(int departureStationId, int arrivalStationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.DepartureStation)
            .Include(r => r.ArrivalStation)
            .Where(r => r.DepartureStationId == departureStationId && 
                       r.ArrivalStationId == arrivalStationId && 
                       r.IsActive)
            .ToListAsync(cancellationToken);
    }
}