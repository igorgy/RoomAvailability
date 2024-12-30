namespace RoomAvailability
{
    internal static class Constants
    {
        // The date format used in the input for the console application.
        public const string DateInputPattern = "yyyyMMdd";

        // Minimum booking days allowed.
        public const int MinBookingSpan = 1;

        // Maximum number of days ahead to query for availability allowed.
        public const int MaxAheadAvailability = 365 * 5;
    }
}
