namespace RoomAvailability.Helpers
{
    /// <summary>
    /// Service responsible for handling time related operations
    /// </summary>
    public interface IDateService
    {
        /// <summary>
        /// Gets the current date 
        /// </summary>
        /// <returns>Current date</returns>
        DateTimeOffset GetCurrentDate();

        /// <summary>
        /// Parses the date string
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Date</returns>
        DateTimeOffset ParseDate(string date);
    }
}
