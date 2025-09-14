using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainTicketBookingSystem.Domain.Entities;

namespace TrainTicketBookingSystem.Infrastructure.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.DepartureTime)
            .IsRequired();

        builder.Property(s => s.ArrivalTime)
            .IsRequired();

        builder.Property(s => s.EconomyPrice)
            .HasPrecision(10, 2);

        builder.Property(s => s.BusinessPrice)
            .HasPrecision(10, 2);

        builder.Property(s => s.FirstClassPrice)
            .HasPrecision(10, 2);

        builder.Ignore(s => s.DomainEvents);

        // Relationships
        builder.HasOne(s => s.Train)
            .WithMany(t => t.Schedules)
            .HasForeignKey(s => s.TrainId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Route)
            .WithMany(r => r.Schedules)
            .HasForeignKey(s => s.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Bookings)
            .WithOne(b => b.Schedule)
            .HasForeignKey(b => b.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SeatNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.Class)
            .HasConversion<int>();

        builder.Ignore(s => s.DomainEvents);

        // Composite unique index
        builder.HasIndex(s => new { s.TrainId, s.SeatNumber })
            .IsUnique();

        // Relationships
        builder.HasOne(s => s.Train)
            .WithMany(t => t.Seats)
            .HasForeignKey(s => s.TrainId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.BookingSeats)
            .WithOne(bs => bs.Seat)
            .HasForeignKey(bs => bs.SeatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}