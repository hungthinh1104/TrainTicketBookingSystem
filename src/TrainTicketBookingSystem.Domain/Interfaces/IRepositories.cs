using TrainTicketBookingSystem.Domain.Entities;

namespace TrainTicketBookingSystem.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

public interface IStationRepository : IRepository<Station>
{
    Task<Station?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Station>> GetByCityAsync(string city, CancellationToken cancellationToken = default);
}

public interface ITrainRepository : IRepository<Train>
{
    Task<Train?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
    Task<IEnumerable<Train>> GetActiveTrainsAsync(CancellationToken cancellationToken = default);
}

public interface IRouteRepository : IRepository<Route>
{
    Task<IEnumerable<Route>> GetByStationsAsync(int departureStationId, int arrivalStationId, CancellationToken cancellationToken = default);
}

public interface IScheduleRepository : IRepository<Schedule>
{
    Task<IEnumerable<Schedule>> SearchSchedulesAsync(int departureStationId, int arrivalStationId, DateTime departureDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Schedule>> GetByTrainIdAsync(int trainId, CancellationToken cancellationToken = default);
}

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Booking?> GetByReferenceAsync(string reference, CancellationToken cancellationToken = default);
    Task<string> GenerateBookingReferenceAsync(CancellationToken cancellationToken = default);
}

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
}

public interface ISeatRepository : IRepository<Seat>
{
    Task<IEnumerable<Seat>> GetByTrainIdAsync(int trainId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int scheduleId, CancellationToken cancellationToken = default);
}

public interface IPassengerRepository : IRepository<Passenger>
{
    Task<IEnumerable<Passenger>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
}

public interface IBookingSeatRepository : IRepository<BookingSeat>
{
    Task<IEnumerable<BookingSeat>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingSeat>> GetByScheduleIdAsync(int scheduleId, CancellationToken cancellationToken = default);
}