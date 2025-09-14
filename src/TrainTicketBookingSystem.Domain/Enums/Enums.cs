namespace TrainTicketBookingSystem.Domain.Enums;

public enum UserRole
{
    User = 1,
    Admin = 2
}

public enum BookingStatus
{
    Pending = 1,
    Confirmed = 2,
    Cancelled = 3,
    Completed = 4
}

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Refunded = 5
}

public enum PaymentMethod
{
    CreditCard = 1,
    DebitCard = 2,
    PayPal = 3,
    BankTransfer = 4
}

public enum SeatClass
{
    Economy = 1,
    Business = 2,
    FirstClass = 3
}

public enum TrainStatus
{
    Active = 1,
    Inactive = 2,
    Maintenance = 3
}