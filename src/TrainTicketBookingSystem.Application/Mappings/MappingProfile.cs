using AutoMapper;
using TrainTicketBookingSystem.Application.DTOs;
using TrainTicketBookingSystem.Domain.Entities;

namespace TrainTicketBookingSystem.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

        CreateMap<Station, StationDto>();
        CreateMap<Route, RouteDto>();
        CreateMap<Train, TrainDto>();
        CreateMap<Schedule, ScheduleDto>();
        CreateMap<Seat, SeatDto>();

        CreateMap<Booking, BookingDto>();
        CreateMap<CreateBookingDto, Booking>()
            .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => Domain.Enums.BookingStatus.Pending))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Passenger, PassengerDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age));

        CreateMap<CreatePassengerDto, Passenger>();

        CreateMap<Payment, PaymentDto>();
        CreateMap<CreatePaymentDto, Payment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => Domain.Enums.PaymentStatus.Pending))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}