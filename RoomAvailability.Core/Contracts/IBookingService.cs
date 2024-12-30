namespace RoomAvailability.Core.Contracts
{
    /// <summary>
    /// A service responsible for booking related operations.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Gets the capacity of the hotel for the specified room type.
        /// </summary>
        /// <param name="hotelId">Hotel Id</param>
        /// <param name="roomType">Room type</param>
        /// <returns>Hotel capacity</returns>
        Task<int> GetHotelCapacity(string hotelId, string roomType);

        /// <summary>
        /// Gets a number of bookings for the specified room type and date range.
        /// </summary>
        /// <param name="hotelId">Hotel Id</param>
        /// <param name="roomType">Room type</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Count of bookings</returns>
        Task<int> GetBookingsCount(string hotelId, string roomType, DateTimeOffset startDate, DateTimeOffset endDate);

        /// <summary>
        /// Gets a daily number of bookings for the specified room type and date range.
        /// </summary>
        /// <param name="hotelId">Hotel Id</param>
        /// <param name="roomType">Room type</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Daily count of bookings</returns>
        Task<int[]> GetDailyBookingsCount(string hotelId, string roomType, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
