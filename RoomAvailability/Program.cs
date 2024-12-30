using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoomAvailability;
using RoomAvailability.Services;

try
{
    var app = new CommandLineApplication
    {
        Name = "Room Availability",
        Description = "An utility to manage hotel room availability and reservations.",
    };
    app.HelpOption("-?|-h|--help");

    var hotelsOption = app.Option("--hotels <FileName>", "Hotels", CommandOptionType.SingleValue);
    var bookingsOption = app.Option("--bookings <FileName>", "Bookings", CommandOptionType.SingleValue);

    app.OnExecute(async () =>
    {
        var hotels = hotelsOption.Value();
        var bookings = bookingsOption.Value();

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(hotels))
        {
            errors.Add("Hotels FileName is not specified.");
        }

        if (string.IsNullOrWhiteSpace(bookings))
        {
            errors.Add("Bookings FileName is not specified.");
        }

        if (errors.Count > 0)
        {
            app.ShowHelp();

            foreach (var error in errors)
            {
                await Console.Error.WriteLineAsync($"ERROR: {error}");
            }

            return -1;
        }

        // Set up the application host
        var host = Host.CreateDefaultBuilder()
        .ConfigureRoomAvailabilityServices()
        .Build();

        // Execute the room availability service
        var roomAvalService = host.Services.GetRequiredService<IRoomAvailabilityService>();
        await roomAvalService.Execute(hotels, bookings);

        return 0;
    });

    app.Execute(args);
}
catch (Exception ex)
{
    await Console.Error.WriteLineAsync(ex.Message);
}