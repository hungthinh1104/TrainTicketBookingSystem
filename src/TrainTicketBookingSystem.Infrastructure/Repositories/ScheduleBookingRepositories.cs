using Microsoft.EntityFrameworkCore;
using TrainTicketBookingSystem.Domain.Entities;
using TrainTicketBookingSystem.Domain.Interfaces;
using TrainTicketBookingSystem.Infrastructure.Data;

namespace TrainTicketBookingSystem.Infrastructure.Repositories;

public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Schedule>> SearchSchedulesAsync(int departureStationId, int arrivalStationId, DateTime departureDate, CancellationToken cancellationToken = default)
    {
        var startDate = departureDate.Date;
        var endDate = startDate.AddDays(1);

        return await _dbSet
            .Include(s => s.Train)
            .Include(s => s.Route)
                .ThenInclude(r => r.DepartureStation)
            .Include(s => s.Route)
                .ThenInclude(r => r.ArrivalStation)
            .Where(s => s.Route.DepartureStationId == departureStationId &&
                       s.Route.ArrivalStationId == arrivalStationId &&
                       s.DepartureTime >= startDate &&
                       s.DepartureTime < endDate &&
                       s.AvailableSeats > 0)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Schedule>> GetByTrainIdAsync(int trainId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Route)
                .ThenInclude(r => r.DepartureStation)
            .Include(s => s.Route)
                .ThenInclude(r => r.ArrivalStation)
            .Where(s => s.TrainId == trainId)
            .OrderBy(s => s.DepartureTime)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Schedule?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Train)
            .Include(s => s.Route)
                .ThenInclude(r => r.DepartureStation)
            .Include(s => s.Route)
                .ThenInclude(r => r.ArrivalStation)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Train)
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.DepartureStation)
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.ArrivalStation)
            .Include(b => b.Passengers)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .Include(b => b.Payment)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Booking?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.User)
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Train)
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.DepartureStation)
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.ArrivalStation)
            .Include(b => b.Passengers)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.BookingReference == reference, cancellationToken);
    }

    public async Task<string> GenerateBookingReferenceAsync(CancellationToken cancellationToken = default)
    {
        string reference;
        bool exists;
        
        do
        {
            reference = GenerateReference();
            exists = await _dbSet.AnyAsync(b => b.BookingReference == reference, cancellationToken);
        }
        while (exists);

        return reference;
    }

    private static string GenerateReference()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public override async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.User)
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Train)
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.DepartureStation)
            .Include(b => b.Schedule)
                .ThenInclude(s => s.Route)
                    .ThenInclude(r => r.ArrivalStation)
            .Include(b => b.Passengers)
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .Include(b => b.Payment)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}