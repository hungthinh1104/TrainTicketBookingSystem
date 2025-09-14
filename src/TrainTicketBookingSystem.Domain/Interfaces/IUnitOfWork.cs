namespace TrainTicketBookingSystem.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IStationRepository Stations { get; }
    ITrainRepository Trains { get; }
    IRouteRepository Routes { get; }
    IScheduleRepository Schedules { get; }
    IBookingRepository Bookings { get; }
    IPaymentRepository Payments { get; }
    ISeatRepository Seats { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}