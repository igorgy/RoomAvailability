namespace RoomAvailability.Data.Entity
{
    /// <summary>
    /// Room entity.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Hotel identifier.
        /// </summary>
        public string HotelId { get; set; }

        /// <summary>
        /// Room type.
        /// </summary>

        public string RoomType { get; set; }

        /// <summary>
        /// Count of the rooms of the type specified in the hotel.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Collection of bookings for the room type.
        /// </summary>
        public IList<Booking> Bookings { get; set; }
    }
}
