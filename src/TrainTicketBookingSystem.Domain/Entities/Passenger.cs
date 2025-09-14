namespace TrainTicketBookingSystem.Domain.Entities;

public class Passenger : BaseEntity
{
    public int BookingId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string IdentityNumber { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Navigation properties
    public virtual Booking Booking { get; set; } = null!;
    public virtual ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();

    public string FullName => $"{FirstName} {LastName}";
    public int Age => DateTime.Today.Year - DateOfBirth.Year;
}