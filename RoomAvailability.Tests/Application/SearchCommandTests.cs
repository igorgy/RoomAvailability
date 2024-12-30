using Moq;
using RoomAvailability.Commands;
using RoomAvailability.Core.Contracts;
using RoomAvailability.Helpers;

namespace RoomAvailability.Tests.Application
{
    public class SearchCommandTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<IDateService> _mockDateService;
        private readonly SearchCommand _searchCommand;

        public SearchCommandTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockDateService = new Mock<IDateService>();
            _searchCommand = new SearchCommand(_mockBookingService.Object, _mockDateService.Object);
        }

        [Fact]
        public void Configure_ValidInput_SetsPropertiesAndReturnsTrue()
        {
            // Arrange
            var input = "Search(Hotel123,5,Deluxe)";
            var currentDate = new DateTimeOffset(2023, 10, 1, 0, 0, 0, TimeSpan.Zero);
            _mockDateService.Setup(ds => ds.GetCurrentDate()).Returns(currentDate);

            // Act
            var result = _searchCommand.Configure(input);

            // Assert
            Assert.True(result);
            Assert.Equal("Hotel123", _searchCommand.HotelId);
            Assert.Equal(currentDate, _searchCommand.StartDate);
            Assert.Equal(currentDate.AddDays(5), _searchCommand.EndDate);
            Assert.Equal("Deluxe", _searchCommand.RoomType);
            Assert.True(_searchCommand.CommandReady);
        }

        [Fact]
        public void Configure_InvalidInput_ReturnsFalse()
        {
            // Arrange
            var input = "InvalidInput";

            // Act
            var result = _searchCommand.Configure(input);

            // Assert
            Assert.False(result);
            Assert.False(_searchCommand.CommandReady);
        }

        [Fact]
        public void Configure_AlreadyConfigured_ThrowsInvalidOperationException()
        {
            // Arrange
            var input = "Search(Hotel123,5,Deluxe)";
            var currentDate = new DateTimeOffset(2023, 10, 1, 0, 0, 0, TimeSpan.Zero);
            _mockDateService.Setup(ds => ds.GetCurrentDate()).Returns(currentDate);
            _searchCommand.Configure(input);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _searchCommand.Configure(input));
        }

        [Fact]
        public void Configure_InvalidSpan_ThrowsInvalidOperationException()
        {
            // Arrange
            var input = $"Search(Hotel123,{5*365 + 1},Deluxe)";
            var currentDate = new DateTimeOffset(2023, 10, 1, 0, 0, 0, TimeSpan.Zero);
            _mockDateService.Setup(ds => ds.GetCurrentDate()).Returns(currentDate);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _searchCommand.Configure(input));
        }

        [Fact]
        public async Task Execute_CommandNotConfigured_ThrowsInvalidOperationException()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await foreach (var _ in _searchCommand.Execute()) { }
            });
        }

        [Fact]
        public async Task Execute_ValidConfiguration_ReturnsExpectedResults()
        {
            // Arrange
            var hotelId = "Hotel123";
            var roomType = "Deluxe";
            var startDate = new DateTimeOffset(2023, 10, 1, 0, 0, 0, TimeSpan.Zero);
            var span = 5;
            var endDate = startDate.AddDays(span);
            var totalCountOfRooms = 10;
            var bookedRoomsCount = new int[] { 2, 3, 2, 4, 1 };

            _mockDateService.Setup(ds => ds.GetCurrentDate()).Returns(startDate);
            _mockBookingService.Setup(bs => bs.GetHotelCapacity(hotelId, roomType)).ReturnsAsync(totalCountOfRooms);
            _mockBookingService.Setup(bs => bs.GetDailyBookingsCount(hotelId, roomType, startDate, endDate)).ReturnsAsync(bookedRoomsCount);

            var input = $"Search({hotelId},{span},{roomType})";
            _searchCommand.Configure(input);

            // Act
            var results = new List<string>();
            await foreach (var result in _searchCommand.Execute())
            {
                results.Add(result);
            }

            // Assert
            var expectedResults = new List<string>
            {
                "(20231001-20231002,8)",
                "(20231002-20231003,7)",
                "(20231003-20231004,8)",
                "(20231004-20231005,6)",
                "(20231005-20231006,9)"
            };

            Assert.Equal(expectedResults, results);
        }
    }
}
