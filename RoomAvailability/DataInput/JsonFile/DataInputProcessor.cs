using RoomAvailability.Data;
using RoomAvailability.Data.Entity;
using RoomAvailability.Helpers;
using System.Text.Json;

namespace RoomAvailability.DataInput.JsonFile
{
    /// <summary>
    /// Implementation of <see cref="IDataInputProcessor"/> that reads data from JSON files and populates the database.
    /// </summary>
    public class DataInputProcessor : IDataInputProcessor
    {
        private readonly RoomAvailabilityContext _context;
        private readonly IDateService _dateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomAvailabilityContext"/> class.
        /// </summary>
        /// <param name="roomAvailabilityContext">DB Context to populate with the data from the input</param>
        /// <param name="dateService">Date service</param>
        public DataInputProcessor(
            RoomAvailabilityContext roomAvailabilityContext,
            IDateService dateService)
        {
            _context = roomAvailabilityContext;
            _dateService = dateService;
        }

        /// <inheritdoc/>
        public async Task Process(string hotelsFilePath, string bookingsFilePath)
        {
            CheckFileExists(hotelsFilePath);
            CheckFileExists(bookingsFilePath);

            // Populate Hotels and Rooms
            await using (var readStream = File.Open(hotelsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await foreach (DTO.Hotel item in JsonSerializer.DeserializeAsyncEnumerable<DTO.Hotel>(readStream))
                {
                    _context.Hotels.Add(new Hotel { Id = item.id });
                    _context.Rooms.AddRange(item.rooms.GroupBy(r => r.roomType).Select(g => new Room { HotelId = item.id, RoomType = g.Key, Count = g.Count() }));
                }
            }
            // Populate Bookings
            await using (var readStream = File.Open(bookingsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await foreach (DTO.Booking item in JsonSerializer.DeserializeAsyncEnumerable<DTO.Booking>(readStream))
                {
                    _context.Bookings.Add(new Booking
                    {
                        Id = Guid.NewGuid(),
                        HotelId = item.hotelId,
                        RoomType = item.roomType,
                        ArrivalDate = _dateService.ParseDate(item.arrival),
                        DepartureDate = _dateService.ParseDate(item.departure)
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        private void CheckFileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found at {filePath}");
            }
        }
    }
}
