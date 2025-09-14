using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainTicketBookingSystem.Domain.Entities;

namespace TrainTicketBookingSystem.Infrastructure.Configurations;

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(s => s.Code)
            .IsUnique();

        builder.Property(s => s.City)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.State)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Country)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Latitude)
            .HasPrecision(10, 8);

        builder.Property(s => s.Longitude)
            .HasPrecision(11, 8);

        builder.Ignore(s => s.DomainEvents);

        // Relationships
        builder.HasMany(s => s.DepartureRoutes)
            .WithOne(r => r.DepartureStation)
            .HasForeignKey(r => r.DepartureStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.ArrivalRoutes)
            .WithOne(r => r.ArrivalStation)
            .HasForeignKey(r => r.ArrivalStationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TrainConfiguration : IEntityTypeConfiguration<Train>
{
    public void Configure(EntityTypeBuilder<Train> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Number)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(t => t.Number)
            .IsUnique();

        builder.Property(t => t.Status)
            .HasConversion<int>();

        builder.Ignore(t => t.DomainEvents);

        // Relationships
        builder.HasMany(t => t.Schedules)
            .WithOne(s => s.Train)
            .HasForeignKey(s => s.TrainId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Seats)
            .WithOne(s => s.Train)
            .HasForeignKey(s => s.TrainId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.EstimatedDuration)
            .IsRequired();

        builder.Ignore(r => r.DomainEvents);

        // Relationships
        builder.HasOne(r => r.DepartureStation)
            .WithMany(s => s.DepartureRoutes)
            .HasForeignKey(r => r.DepartureStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ArrivalStation)
            .WithMany(s => s.ArrivalRoutes)
            .HasForeignKey(r => r.ArrivalStationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Schedules)
            .WithOne(s => s.Route)
            .HasForeignKey(s => s.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}