using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using System.Text;
using TrainTicketBookingSystem.Application.Interfaces;
using TrainTicketBookingSystem.Domain.Interfaces;
using TrainTicketBookingSystem.Infrastructure.Caching;
using TrainTicketBookingSystem.Infrastructure.Data;
using TrainTicketBookingSystem.Infrastructure.Repositories;
using TrainTicketBookingSystem.Infrastructure.Services;

namespace TrainTicketBookingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repository Pattern & Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IPricingStrategy, StandardPricingStrategy>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailService, EmailService>();

        // Caching
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();

        // Redis (optional)
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(provider =>
                ConnectionMultiplexer.Connect(redisConnectionString));
        }
        else
        {
            services.AddSingleton<IConnectionMultiplexer>(provider => 
                throw new InvalidOperationException("Redis connection string not configured"));
        }

        // JWT Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
                var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
                var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        // HTTP Context Accessor
        services.AddHttpContextAccessor();

        // Serilog
        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "logs/train-booking-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    shared: true);
        });

        return services;
    }

    public static async Task<IServiceProvider> SeedDataAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await context.Database.MigrateAsync();
        await SeedDatabaseAsync(context);
        
        return serviceProvider;
    }

    private static async Task SeedDatabaseAsync(ApplicationDbContext context)
    {
        if (!context.Stations.Any())
        {
            await SeedStationsAsync(context);
        }

        if (!context.Trains.Any())
        {
            await SeedTrainsAsync(context);
        }

        if (!context.Routes.Any())
        {
            await SeedRoutesAsync(context);
        }

        if (!context.Schedules.Any())
        {
            await SeedSchedulesAsync(context);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedStationsAsync(ApplicationDbContext context)
    {
        var stations = new[]
        {
            new { Name = "New York Penn Station", Code = "NYP", City = "New York", State = "NY", Country = "USA", Lat = 40.750568m, Lng = -73.993519m },
            new { Name = "Washington Union Station", Code = "WAS", City = "Washington", State = "DC", Country = "USA", Lat = 38.897728m, Lng = -77.006058m },
            new { Name = "Philadelphia 30th Street", Code = "PHL", City = "Philadelphia", State = "PA", Country = "USA", Lat = 39.955902m, Lng = -75.182434m },
            new { Name = "Boston South Station", Code = "BOS", City = "Boston", State = "MA", Country = "USA", Lat = 42.352271m, Lng = -71.055242m },
            new { Name = "Chicago Union Station", Code = "CHI", City = "Chicago", State = "IL", Country = "USA", Lat = 41.878876m, Lng = -87.640026m }
        };

        foreach (var station in stations)
        {
            context.Stations.Add(new Domain.Entities.Station
            {
                Name = station.Name,
                Code = station.Code,
                City = station.City,
                State = station.State,
                Country = station.Country,
                Latitude = station.Lat,
                Longitude = station.Lng,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static async Task SeedTrainsAsync(ApplicationDbContext context)
    {
        var trains = new[]
        {
            new { Name = "Acela Express", Number = "ACE001", TotalSeats = 200, Economy = 120, Business = 60, FirstClass = 20 },
            new { Name = "Northeast Regional", Number = "NER002", TotalSeats = 300, Economy = 220, Business = 60, FirstClass = 20 },
            new { Name = "Silver Star", Number = "SIL003", TotalSeats = 350, Economy = 250, Business = 80, FirstClass = 20 },
            new { Name = "Lake Shore Limited", Number = "LSL004", TotalSeats = 280, Economy = 200, Business = 60, FirstClass = 20 }
        };

        foreach (var train in trains)
        {
            var trainEntity = new Domain.Entities.Train
            {
                Name = train.Name,
                Number = train.Number,
                TotalSeats = train.TotalSeats,
                EconomySeats = train.Economy,
                BusinessSeats = train.Business,
                FirstClassSeats = train.FirstClass,
                Status = Domain.Enums.TrainStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            context.Trains.Add(trainEntity);
            await context.SaveChangesAsync(); // Save to get train ID

            // Add seats for this train
            for (int i = 1; i <= train.Economy; i++)
            {
                context.Seats.Add(new Domain.Entities.Seat
                {
                    TrainId = trainEntity.Id,
                    SeatNumber = $"E{i:D3}",
                    Class = Domain.Enums.SeatClass.Economy,
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            for (int i = 1; i <= train.Business; i++)
            {
                context.Seats.Add(new Domain.Entities.Seat
                {
                    TrainId = trainEntity.Id,
                    SeatNumber = $"B{i:D3}",
                    Class = Domain.Enums.SeatClass.Business,
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            for (int i = 1; i <= train.FirstClass; i++)
            {
                context.Seats.Add(new Domain.Entities.Seat
                {
                    TrainId = trainEntity.Id,
                    SeatNumber = $"F{i:D3}",
                    Class = Domain.Enums.SeatClass.FirstClass,
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }

    private static async Task SeedRoutesAsync(ApplicationDbContext context)
    {
        var stations = await context.Stations.ToListAsync();
        var nyp = stations.First(s => s.Code == "NYP");
        var was = stations.First(s => s.Code == "WAS");
        var phl = stations.First(s => s.Code == "PHL");
        var bos = stations.First(s => s.Code == "BOS");
        var chi = stations.First(s => s.Code == "CHI");

        var routes = new[]
        {
            new { From = nyp.Id, To = was.Id, Distance = 230, Duration = TimeSpan.FromHours(3) },
            new { From = was.Id, To = nyp.Id, Distance = 230, Duration = TimeSpan.FromHours(3) },
            new { From = nyp.Id, To = phl.Id, Distance = 95, Duration = TimeSpan.FromHours(1.5) },
            new { From = phl.Id, To = nyp.Id, Distance = 95, Duration = TimeSpan.FromHours(1.5) },
            new { From = nyp.Id, To = bos.Id, Distance = 215, Duration = TimeSpan.FromHours(3.5) },
            new { From = bos.Id, To = nyp.Id, Distance = 215, Duration = TimeSpan.FromHours(3.5) },
            new { From = nyp.Id, To = chi.Id, Distance = 790, Duration = TimeSpan.FromHours(19) },
            new { From = chi.Id, To = nyp.Id, Distance = 790, Duration = TimeSpan.FromHours(19) }
        };

        foreach (var route in routes)
        {
            context.Routes.Add(new Domain.Entities.Route
            {
                DepartureStationId = route.From,
                ArrivalStationId = route.To,
                DistanceKm = route.Distance,
                EstimatedDuration = route.Duration,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static async Task SeedSchedulesAsync(ApplicationDbContext context)
    {
        var trains = await context.Trains.ToListAsync();
        var routes = await context.Routes.ToListAsync();

        var baseDate = DateTime.Today.AddDays(1);

        for (int day = 0; day < 30; day++)
        {
            var currentDate = baseDate.AddDays(day);

            foreach (var route in routes.Take(4)) // Limit for demo
            {
                var train = trains[Random.Shared.Next(trains.Count)];
                var departureTime = currentDate.AddHours(8 + Random.Shared.Next(12));
                var arrivalTime = departureTime.Add(route.EstimatedDuration);

                context.Schedules.Add(new Domain.Entities.Schedule
                {
                    TrainId = train.Id,
                    RouteId = route.Id,
                    DepartureTime = departureTime,
                    ArrivalTime = arrivalTime,
                    EconomyPrice = 50 + Random.Shared.Next(100),
                    BusinessPrice = 100 + Random.Shared.Next(150),
                    FirstClassPrice = 200 + Random.Shared.Next(200),
                    AvailableSeats = train.TotalSeats,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }
}