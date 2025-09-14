using AutoMapper;
using MediatR;
using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Application.Interfaces;
using TrainTicketBookingSystem.Domain.Entities;
using TrainTicketBookingSystem.Domain.Exceptions;
using TrainTicketBookingSystem.Domain.Interfaces;

namespace TrainTicketBookingSystem.Application.Commands.Users;

public record RegisterUserCommand(CreateUserDto UserDto) : IRequest<UserDto>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;

    public RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordService passwordService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordService = passwordService;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.UserDto.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new DomainException("User with this email already exists.");
        }

        // Create new user
        var user = _mapper.Map<User>(request.UserDto);
        user.PasswordHash = _passwordService.HashPassword(request.UserDto.Password);

        var createdUser = await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDto>(createdUser);
    }
}

public record LoginUserCommand(LoginDto LoginDto) : IRequest<LoginResponseDto>;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;
    private readonly IAuthService _authService;

    public LoginUserCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordService passwordService,
        IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordService = passwordService;
        _authService = authService;
    }

    public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.LoginDto.Email, cancellationToken);
        
        if (user == null || !_passwordService.VerifyPassword(request.LoginDto.Password, user.PasswordHash))
        {
            throw new DomainException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new DomainException("User account is inactive.");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = _mapper.Map<UserDto>(user);
        var token = await _authService.GenerateTokenAsync(userDto, cancellationToken);

        return new LoginResponseDto
        {
            Token = token,
            User = userDto,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }
}