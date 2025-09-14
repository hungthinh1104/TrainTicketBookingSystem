using Microsoft.EntityFrameworkCore.Storage;
using TrainTicketBookingSystem.Domain.Interfaces;
using TrainTicketBookingSystem.Infrastructure.Data;
using TrainTicketBookingSystem.Infrastructure.Repositories;

namespace TrainTicketBookingSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IUserRepository? _users;
    private IStationRepository? _stations;
    private ITrainRepository? _trains;
    private IRouteRepository? _routes;
    private IScheduleRepository? _schedules;
    private IBookingRepository? _bookings;
    private IPaymentRepository? _payments;
    private ISeatRepository? _seats;
    private IPassengerRepository? _passengers;
    private IBookingSeatRepository? _bookingSeats;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users =>
        _users ??= new UserRepository(_context);

    public IStationRepository Stations =>
        _stations ??= new StationRepository(_context);

    public ITrainRepository Trains =>
        _trains ??= new TrainRepository(_context);

    public IRouteRepository Routes =>
        _routes ??= new RouteRepository(_context);

    public IScheduleRepository Schedules =>
        _schedules ??= new ScheduleRepository(_context);

    public IBookingRepository Bookings =>
        _bookings ??= new BookingRepository(_context);

    public IPaymentRepository Payments =>
        _payments ??= new PaymentRepository(_context);

    public ISeatRepository Seats =>
        _seats ??= new SeatRepository(_context);

    public IPassengerRepository Passengers =>
        _passengers ??= new PassengerRepository(_context);

    public IBookingSeatRepository BookingSeats =>
        _bookingSeats ??= new BookingSeatRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}