using TrainTicketBookingSystem.Application.Interfaces;
using TrainTicketBookingSystem.Domain.Enums;

namespace TrainTicketBookingSystem.Infrastructure.Services;

public class StandardPricingStrategy : IPricingStrategy
{
    public decimal CalculatePrice(decimal basePrice, SeatClass seatClass, DateTime bookingDate, DateTime departureDate)
    {
        decimal price = basePrice;

        // Apply class multiplier
        price *= GetClassMultiplier(seatClass);

        // Apply advance booking discount
        var daysInAdvance = (departureDate.Date - bookingDate.Date).Days;
        price *= GetAdvanceBookingMultiplier(daysInAdvance);

        // Apply peak time surcharge
        price *= GetPeakTimeMultiplier(departureDate);

        return Math.Round(price, 2);
    }

    private static decimal GetClassMultiplier(SeatClass seatClass)
    {
        return seatClass switch
        {
            SeatClass.Economy => 1.0m,
            SeatClass.Business => 1.5m,
            SeatClass.FirstClass => 2.0m,
            _ => 1.0m
        };
    }

    private static decimal GetAdvanceBookingMultiplier(int daysInAdvance)
    {
        return daysInAdvance switch
        {
            >= 30 => 0.8m,  // 20% discount
            >= 14 => 0.9m,  // 10% discount
            >= 7 => 0.95m,  // 5% discount
            _ => 1.0m       // No discount
        };
    }

    private static decimal GetPeakTimeMultiplier(DateTime departureTime)
    {
        var hour = departureTime.Hour;
        var dayOfWeek = departureTime.DayOfWeek;

        // Weekend surcharge
        if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
        {
            return 1.1m; // 10% surcharge
        }

        // Peak hours surcharge (7-9 AM, 5-7 PM on weekdays)
        if ((hour >= 7 && hour <= 9) || (hour >= 17 && hour <= 19))
        {
            return 1.15m; // 15% surcharge
        }

        return 1.0m; // No surcharge
    }
}