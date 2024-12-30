namespace RoomAvailability.Data.Entity
{
    /// <summary>
    /// A booking entity.
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Hotel identifier.
        /// </summary>
        public string HotelId { get; set; }

        /// <summary>
        /// Room type.
        /// </summary>
        public string RoomType { get; set; }

        /// <summary>
        /// Room entity.
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// Arrival date.
        /// </summary>
        public DateTimeOffset ArrivalDate { get; set; }

        /// <summary>
        /// Departure date.
        /// </summary>
        public DateTimeOffset DepartureDate { get; set; }
    }
}
