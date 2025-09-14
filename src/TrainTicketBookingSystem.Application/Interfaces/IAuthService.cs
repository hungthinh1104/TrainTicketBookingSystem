using TrainTicketBookingSystem.Application.DTOs;

namespace TrainTicketBookingSystem.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<UserDto> RegisterAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default);
    Task<string> GenerateTokenAsync(UserDto user, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface ICurrentUserService
{
    int UserId { get; }
    string Email { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
}