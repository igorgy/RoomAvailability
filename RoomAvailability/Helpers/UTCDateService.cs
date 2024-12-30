using System.Globalization;

namespace RoomAvailability.Helpers
{
    public class UTCDateService : IDateService
    {
        /// <summary>
        /// Gets the current UTC date 
        /// </summary>
        /// <returns>Current UTC date</returns>
        public DateTimeOffset GetCurrentDate()
        {
            return DateTimeOffset.UtcNow.Date;
        }

        /// <summary>
        /// Parses the date string as it is in UTC
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Date in UTC</returns>
        public DateTimeOffset ParseDate(string date)
        {
            return DateTimeOffset.ParseExact(date, Constants.DateInputPattern, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }
    }
}
