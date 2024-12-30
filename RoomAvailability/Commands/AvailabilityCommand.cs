using RoomAvailability.Core.Contracts;
using RoomAvailability.Helpers;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RoomAvailability.Commands
{
    /// <summary>
    /// Command that gets availability count for the specified room type and date range.
    /// </summary>
    public class AvailabilityCommand : ICommand
    {
        private readonly string _pattern = "(Availability)\\(([A-Za-z0-9]+),(\\d{8}|\\d{8}-\\d{8}),([A-Za-z0-9]+)\\)";

        /// <summary>
        /// Hotel identifier.
        /// </summary>
        public string HotelId { get; private set; }

        /// <summary>
        /// Start date.
        /// </summary>
        public DateTimeOffset StartDate { get; private set; }

        /// <summary>
        /// Eend date.
        /// </summary>
        public DateTimeOffset EndDate { get; private set; }

        /// <summary>
        /// Room type.
        /// </summary>
        public string RoomType { get; private set; }

        /// <summary>
        /// Command ready flag.
        /// </summary>
        public bool CommandReady { get; private set; }

        private readonly IBookingService _bookingService;
        private readonly IDateService _dateService;

        /// <summary>
        /// Constructor of the availability command.
        /// </summary>
        /// <param name="bookingService">Booking service</param>
        /// <param name="dateService">Date service</param>
        public AvailabilityCommand(
            IBookingService bookingService,
            IDateService dateService)
        {
            _bookingService = bookingService;
            _dateService = dateService;
        }

        /// <inheritdoc/>
        public bool Configure(string input)
        {
            if(CommandReady)
            {
                throw new InvalidOperationException("Command is already configured.");
            }

            Regex regex = new Regex(_pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(input))
            {
                var match = regex.Match(input);
                HotelId = match.Groups[2].Value;
                if (match.Groups[3].Value.Contains("-"))
                {
                    var dates = match.Groups[3].Value.Split('-');
                    StartDate = _dateService.ParseDate(dates[0]);
                    EndDate = _dateService.ParseDate(dates[1]);
                    if (StartDate >= EndDate)
                    {
                        throw new InvalidOperationException("Start date should be before end date.");
                    }
                }
                else
                {
                    StartDate = _dateService.ParseDate(match.Groups[3].Value);
                    EndDate = StartDate.AddDays(1);
                }
                RoomType = match.Groups[4].Value;

                CommandReady = true;
                return true;
            }
            CommandReady = false;
            return false;
        }

        /// <summary>
        /// Executes the availability command.
        /// </summary>
        /// <returns>The availability count for the specified room type and date range.</returns>
        public async IAsyncEnumerable<string> Execute()
        {
            if (!CommandReady)
            {
                throw new InvalidOperationException("Command is not configured.");
            }

            var bookedRoomsCount = await _bookingService.GetBookingsCount(HotelId, RoomType, StartDate, EndDate);
            var totalCountOfRooms = await _bookingService.GetHotelCapacity(HotelId, RoomType);

            var availableRoomsCount = totalCountOfRooms - bookedRoomsCount;

            yield return availableRoomsCount.ToString();
        }
    }
}
