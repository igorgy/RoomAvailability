using Microsoft.EntityFrameworkCore;
using RoomAvailability.Core.Contracts;
using RoomAvailability.Data;
using RoomAvailability.Data.Entity;

namespace RoomAvailability.Core.Services
{
    /// <inheritdoc/>
    public class BookingService : IBookingService
    {
        private readonly RoomAvailabilityContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="roomAvailabilityContext">DB context</param>
        public BookingService(RoomAvailabilityContext roomAvailabilityContext) 
        {
            _context = roomAvailabilityContext;
        }

        /// <inheritdoc/>
        public async Task<int> GetHotelCapacity(string hotelId, string roomType)
        {
            return await _context
                .Rooms
                .AsNoTracking()
                .Where(r => r.HotelId == hotelId && r.RoomType == roomType)
                .Select(r => r.Count)
                .SingleOrDefaultAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetBookingsCount(string hotelId, string roomType, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await GetBookings(hotelId, roomType, startDate, endDate)
                .CountAsync();
        }

        /// <inheritdoc/>
        public async Task<int[]> GetDailyBookingsCount(string hotelId, string roomType, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var activeBookings = await GetBookings(hotelId, roomType, startDate, endDate)
                .Select(b => new
                {
                    b.ArrivalDate,
                    b.DepartureDate
                })
                .ToListAsync();

            int span = (endDate - startDate).Days;
            int[] dailyBookingsCount = new int[span];

            foreach (var item in activeBookings)
            {
                var start = Math.Max(0, (item.ArrivalDate - startDate).Days);
                var end = Math.Min(span, (item.DepartureDate - startDate).Days);

                for (int i = start; i < end; i++)
                {
                    dailyBookingsCount[i]++;
                }
            }

            return dailyBookingsCount;
        }

        private IQueryable<Booking> GetBookings(string hotelId, string roomType, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return _context
                .Bookings
                .AsNoTracking()
                .Where(b => b.HotelId == hotelId && b.RoomType == roomType &&
                            ((startDate >= b.ArrivalDate && startDate < b.DepartureDate) ||
                            (endDate > b.ArrivalDate && endDate <= b.DepartureDate) ||
                            (startDate <= b.ArrivalDate && endDate >= b.DepartureDate)));
        }
    }
}
