using FluentValidation;
using TrainTicketBookingSystem.Application.Commands.Bookings;
using TrainTicketBookingSystem.Application.DTOs;

namespace TrainTicketBookingSystem.Application.Validators;

public class CreatePassengerDtoValidator : AbstractValidator<CreatePassengerDto>
{
    public CreatePassengerDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
            .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("Date of birth must be within the last 120 years.");

        RuleFor(x => x.IdentityNumber)
            .NotEmpty().WithMessage("Identity number is required.")
            .MaximumLength(20).WithMessage("Identity number must not exceed 20 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("A valid phone number is required.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email address is required.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.ScheduleId)
            .GreaterThan(0).WithMessage("A valid schedule ID is required.");

        RuleFor(x => x.Passengers)
            .NotEmpty().WithMessage("At least one passenger is required.")
            .Must(p => p.Count <= 6).WithMessage("Maximum 6 passengers allowed per booking.");

        RuleForEach(x => x.Passengers)
            .SetValidator(new CreatePassengerDtoValidator());

        RuleFor(x => x.SeatIds)
            .NotEmpty().WithMessage("At least one seat must be selected.");

        RuleFor(x => x)
            .Must(x => x.Passengers.Count == x.SeatIds.Count)
            .WithMessage("Number of passengers must match number of selected seats.");

        RuleFor(x => x.SeatClass)
            .IsInEnum().WithMessage("A valid seat class is required.");
    }
}

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.BookingDto).SetValidator(new CreateBookingDtoValidator());
    }
}

public class SearchScheduleDtoValidator : AbstractValidator<SearchScheduleDto>
{
    public SearchScheduleDtoValidator()
    {
        RuleFor(x => x.DepartureStationId)
            .GreaterThan(0).WithMessage("A valid departure station is required.");

        RuleFor(x => x.ArrivalStationId)
            .GreaterThan(0).WithMessage("A valid arrival station is required.");

        RuleFor(x => x)
            .Must(x => x.DepartureStationId != x.ArrivalStationId)
            .WithMessage("Departure and arrival stations must be different.");

        RuleFor(x => x.DepartureDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Departure date must be today or in the future.")
            .LessThanOrEqualTo(DateTime.Today.AddMonths(6))
            .WithMessage("Departure date must be within the next 6 months.");

        RuleFor(x => x.PreferredClass)
            .IsInEnum().WithMessage("A valid seat class is required.")
            .When(x => x.PreferredClass.HasValue);
    }
}