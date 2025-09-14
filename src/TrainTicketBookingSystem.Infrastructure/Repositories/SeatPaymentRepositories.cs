using Microsoft.EntityFrameworkCore;
using TrainTicketBookingSystem.Domain.Entities;
using TrainTicketBookingSystem.Domain.Interfaces;
using TrainTicketBookingSystem.Infrastructure.Data;

namespace TrainTicketBookingSystem.Infrastructure.Repositories;

public class SeatRepository : Repository<Seat>, ISeatRepository
{
    public SeatRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Seat>> GetByTrainIdAsync(int trainId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.TrainId == trainId)
            .OrderBy(s => s.SeatNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int scheduleId, CancellationToken cancellationToken = default)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Train)
            .FirstOrDefaultAsync(s => s.Id == scheduleId, cancellationToken);

        if (schedule == null)
            return new List<Seat>();

        var bookedSeatIds = await _context.BookingSeats
            .Include(bs => bs.Booking)
            .Where(bs => bs.Booking.ScheduleId == scheduleId && 
                        bs.Booking.Status != Domain.Enums.BookingStatus.Cancelled)
            .Select(bs => bs.SeatId)
            .ToListAsync(cancellationToken);

        return await _dbSet
            .Where(s => s.TrainId == schedule.TrainId && 
                       !bookedSeatIds.Contains(s.Id) && 
                       s.IsAvailable)
            .OrderBy(s => s.SeatNumber)
            .ToListAsync(cancellationToken);
    }
}

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Payment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Booking)
                .ThenInclude(b => b.Schedule)
                    .ThenInclude(s => s.Train)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
    }
}

public class PassengerRepository : Repository<Passenger>, IPassengerRepository
{
    public PassengerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Passenger>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.BookingId == bookingId)
            .ToListAsync(cancellationToken);
    }
}

public class BookingSeatRepository : Repository<BookingSeat>, IBookingSeatRepository
{
    public BookingSeatRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BookingSeat>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(bs => bs.Seat)
            .Include(bs => bs.Passenger)
            .Where(bs => bs.BookingId == bookingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BookingSeat>> GetByScheduleIdAsync(int scheduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(bs => bs.Booking)
            .Include(bs => bs.Seat)
            .Include(bs => bs.Passenger)
            .Where(bs => bs.Booking.ScheduleId == scheduleId)
            .ToListAsync(cancellationToken);
    }
}