using RoomAvailability.Core.Contracts;
using RoomAvailability.Helpers;
using System.Text.RegularExpressions;

namespace RoomAvailability.Commands
{
    /// <summary>
    /// Command that searches and provides date ranges and availability where the room is available.
    /// </summary>
    public class SearchCommand : ICommand
    {
        private readonly string _pattern = "(Search)\\(([A-Za-z0-9]+),([0-9]+),([A-Za-z0-9]+)\\)";
        private int _span;

        /// <summary>
        /// Hotel identifier.
        /// </summary>
        public string HotelId { get; private set; }

        /// <summary>
        /// Search start date.
        /// </summary>
        public DateTimeOffset StartDate { get; private set; }

        /// <summary>
        /// Search end date.
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
        /// Constructor of the search command.
        /// </summary>
        /// <param name="bookingService">Booking service</param>
        /// <param name="dateService">Date service</param>
        public SearchCommand(
            IBookingService bookingService,
            IDateService dateService)
        {
            _bookingService = bookingService;
            _dateService = dateService;
        }

        /// <inheritdoc/>
        public bool Configure(string input)
        {
            if (CommandReady)
            {
                throw new InvalidOperationException("Command is already configured.");
            }

            Regex regex = new Regex(_pattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(input))
            {
                var match = regex.Match(input);
                HotelId = match.Groups[2].Value;
                StartDate = _dateService.GetCurrentDate();
                _span = int.Parse(match.Groups[3].Value);
                if (_span < Constants.MinBookingSpan || _span > Constants.MaxAheadAvailability)
                {
                    throw new InvalidOperationException($"The number of days ahead should be between {Constants.MinBookingSpan} and {Constants.MaxAheadAvailability}.");
                }
                EndDate = StartDate.AddDays(_span);
                RoomType = match.Groups[4].Value;

                CommandReady = true;
                return true;
            }
            CommandReady = false;
            return false;
        }

        /// <summary>
        /// Executes the search command.
        /// </summary>
        /// <returns>A collection of string representation of date ranges and availability where the room is available</returns>
        public async IAsyncEnumerable<string> Execute()
        {
            if (!CommandReady)
            {
                throw new InvalidOperationException("Command is not configured.");
            }

            var totalCountOfRooms = await _bookingService.GetHotelCapacity(HotelId, RoomType);
            var bookedRoomsCount = await _bookingService.GetDailyBookingsCount(HotelId, RoomType, StartDate, EndDate);

            var availableRoomsCount = bookedRoomsCount.Select(booked => totalCountOfRooms - booked).ToArray();

            var currInd = 0;
            var currStartInd = 0;
            var currVal = availableRoomsCount[currStartInd];
            while (++currInd < _span)
            {
                if (currVal != availableRoomsCount[currInd])
                {
                    yield return $"({StartDate.AddDays(currStartInd).ToString(Constants.DateInputPattern)}-{StartDate.AddDays(currInd).ToString(Constants.DateInputPattern)},{currVal})";
                    currVal = availableRoomsCount[currInd];
                    currStartInd = currInd;
                }
            }
            yield return $"({StartDate.AddDays(currStartInd).ToString(Constants.DateInputPattern)}-{StartDate.AddDays(currInd).ToString(Constants.DateInputPattern)},{currVal})";
        }
    }
}
