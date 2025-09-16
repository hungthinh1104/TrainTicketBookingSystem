using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainTicketBookingSystem.Domain.Entities;

namespace TrainTicketBookingSystem.Infrastructure.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BookingReference)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(b => b.BookingReference)
            .IsUnique();

        builder.Property(b => b.TotalAmount)
            .HasPrecision(10, 2);

        builder.Property(b => b.Status)
            .HasConversion<int>();

        builder.Property(b => b.BookingDate)
            .IsRequired();

        builder.Ignore(b => b.DomainEvents);

        // Relationships
        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Schedule)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.BookingSeats)
            .WithOne(bs => bs.Booking)
            .HasForeignKey(bs => bs.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Passengers)
            .WithOne(p => p.Booking)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Payment)
            .WithOne(p => p.Booking)
            .HasForeignKey<Payment>(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BookingSeatConfiguration : IEntityTypeConfiguration<BookingSeat>
{
    public void Configure(EntityTypeBuilder<BookingSeat> builder)
    {
        builder.HasKey(bs => bs.Id);

        builder.Ignore(bs => bs.DomainEvents);

        // Composite unique index
        builder.HasIndex(bs => new { bs.BookingId, bs.SeatId })
            .IsUnique();

        // Relationships
        builder.HasOne(bs => bs.Booking)
            .WithMany(b => b.BookingSeats)
            .HasForeignKey(bs => bs.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bs => bs.Seat)
            .WithMany(s => s.BookingSeats)
            .HasForeignKey(bs => bs.SeatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bs => bs.Passenger)
            .WithMany(p => p.BookingSeats)
            .HasForeignKey(bs => bs.PassengerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
{
    public void Configure(EntityTypeBuilder<Passenger> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.IdentityNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Email)
            .HasMaxLength(100);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.Ignore(p => p.FullName);
        builder.Ignore(p => p.Age);
        builder.Ignore(p => p.DomainEvents);

        // Relationships
        builder.HasOne(p => p.Booking)
            .WithMany(b => b.Passengers)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.BookingSeats)
            .WithOne(bs => bs.Passenger)
            .HasForeignKey(bs => bs.PassengerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasPrecision(10, 2);

        builder.Property(p => p.Method)
            .HasConversion<int>();

        builder.Property(p => p.Status)
            .HasConversion<int>();

        builder.Property(p => p.TransactionId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.TransactionId)
            .IsUnique();

        builder.Property(p => p.GatewayResponse)
            .HasMaxLength(1000);

        builder.Ignore(p => p.DomainEvents);

        // Relationships
        builder.HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Booking)
            .WithOne(b => b.Payment)
            .HasForeignKey<Payment>(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}