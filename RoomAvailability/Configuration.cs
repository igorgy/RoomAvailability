using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RoomAvailability.Commands;
using RoomAvailability.Core.Contracts;
using RoomAvailability.Core.Services;
using RoomAvailability.Data;
using RoomAvailability.DataInput.JsonFile;
using RoomAvailability.Helpers;
using RoomAvailability.Services;


namespace RoomAvailability
{
    /// <summary>
    /// Configuration class for RoomAvailability console application.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Configures services for RoomAvailability console application.
        /// </summary>
        /// <param name="builder">Host builder</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureRoomAvailabilityServices(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddTransient<ICommand, AvailabilityCommand>();
                services.AddTransient<ICommand, SearchCommand>();
                services.AddScoped<IBookingService, BookingService>();
                services.AddScoped<IDataInputProcessor, DataInputProcessor>();
                services.AddScoped<IRoomAvailabilityService, RoomAvailabilityService>();
                services.TryAddSingleton<IDateService, UTCDateService>();

                var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
                keepAliveConnection.Open();

                services.AddDbContext<RoomAvailabilityContext>(options =>
                {
                    options.UseSqlite(keepAliveConnection);
                }, ServiceLifetime.Scoped);
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Error);
            });
            return builder;
        }
    }
}
