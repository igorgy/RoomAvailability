using Moq;
using RoomAvailability.Commands;
using RoomAvailability.Core.Contracts;
using RoomAvailability.Helpers;

namespace RoomAvailability.Tests.Application
{
    public class AvailabilityCommandTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly Mock<IDateService> _mockDateService;
        private readonly AvailabilityCommand _command;

        public AvailabilityCommandTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockDateService = new Mock<IDateService>();
            _command = new AvailabilityCommand(_mockBookingService.Object, _mockDateService.Object);
        }

        [Fact]
        public void Configure_ValidInput_SetsPropertiesAndReturnsTrue()
        {
            // Arrange
            string input = "Availability(Hotel123,20230101-20230105,Deluxe)";
            _mockDateService.Setup(ds => ds.ParseDate("20230101")).Returns(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));
            _mockDateService.Setup(ds => ds.ParseDate("20230105")).Returns(new DateTimeOffset(2023, 1, 5, 0, 0, 0, TimeSpan.Zero));

            // Act
            bool result = _command.Configure(input);

            // Assert
            Assert.True(result);
            Assert.Equal("Hotel123", _command.HotelId);
            Assert.Equal(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero), _command.StartDate);
            Assert.Equal(new DateTimeOffset(2023, 1, 5, 0, 0, 0, TimeSpan.Zero), _command.EndDate);
            Assert.Equal("Deluxe", _command.RoomType);
            Assert.True(_command.CommandReady);
        }

        [Fact]
        public void Configure_InvalidInput_ReturnsFalse()
        {
            // Arrange
            string input = "InvalidInput";

            // Act
            bool result = _command.Configure(input);

            // Assert
            Assert.False(result);
            Assert.False(_command.CommandReady);
        }

        [Fact]
        public void Configure_CommandAlreadyConfigured_ThrowsInvalidOperationException()
        {
            // Arrange
            string input = "Availability(Hotel123,20230101-20230105,Deluxe)";
            _mockDateService.Setup(ds => ds.ParseDate("20230101")).Returns(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));
            _mockDateService.Setup(ds => ds.ParseDate("20230105")).Returns(new DateTimeOffset(2023, 1, 5, 0, 0, 0, TimeSpan.Zero));
            _command.Configure(input);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _command.Configure(input));
        }

        [Fact]
        public void Configure_StartDateAfterEndDate_ThrowsInvalidOperationException()
        {
            // Arrange
            string input = "Availability(Hotel123,20230105-20230101,Deluxe)";
            _mockDateService.Setup(ds => ds.ParseDate("20230105")).Returns(new DateTimeOffset(2023, 1, 5, 0, 0, 0, TimeSpan.Zero));
            _mockDateService.Setup(ds => ds.ParseDate("20230101")).Returns(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _command.Configure(input));
        }

        [Fact]
        public async Task Execute_ShouldReturnCorrectAvailabilityCount()
        {
            // Arrange
            string input = "Availability(Hotel123,20230101-20230105,Deluxe)";
            _mockDateService.Setup(ds => ds.ParseDate("20230101")).Returns(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));
            _mockDateService.Setup(ds => ds.ParseDate("20230105")).Returns(new DateTimeOffset(2023, 1, 5, 0, 0, 0, TimeSpan.Zero));
            _mockBookingService.Setup(bs => bs.GetBookingsCount("Hotel123", "Deluxe", It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync(10);
            _mockBookingService.Setup(bs => bs.GetHotelCapacity("Hotel123", "Deluxe")).ReturnsAsync(20);

            _command.Configure(input);

            // Act
            var result = _command.Execute();
            var enumerator = result.GetAsyncEnumerator();
            await enumerator.MoveNextAsync();

            // Assert
            Assert.Equal("10", enumerator.Current);
        }

        [Fact]
        public async Task Execute_ShouldReturnCorrectNegativeAvailabilityCount()
        {
            // Arrange
            string input = "Availability(Hotel123,20230101-20230105,Deluxe)";
            _mockDateService.Setup(ds => ds.ParseDate("20230101")).Returns(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero));
            _mockDateService.Setup(ds => ds.ParseDate("20230105")).Returns(new DateTimeOffset(2023, 1, 5, 0, 0, 0, TimeSpan.Zero));
            _mockBookingService.Setup(bs => bs.GetBookingsCount("Hotel123", "Deluxe", It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).ReturnsAsync(10);
            _mockBookingService.Setup(bs => bs.GetHotelCapacity("Hotel123", "Deluxe")).ReturnsAsync(8);

            _command.Configure(input);

            // Act
            var result = _command.Execute();
            var enumerator = result.GetAsyncEnumerator();
            await enumerator.MoveNextAsync();

            // Assert
            Assert.Equal("-2", enumerator.Current);
        }
    }
}