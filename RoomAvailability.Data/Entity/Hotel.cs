namespace RoomAvailability.Data.Entity
{
    /// <summary>
    /// Hotel entity.
    /// </summary>
    public class Hotel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Rooms collection.
        /// </summary>
        public IList<Room> Rooms { get; set; }
    }
}
