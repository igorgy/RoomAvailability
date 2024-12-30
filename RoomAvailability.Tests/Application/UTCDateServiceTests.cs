using RoomAvailability.Helpers;

namespace RoomAvailability.Tests.Application
{
    public class UTCDateServiceTests
    {
        private readonly UTCDateService _utcDateService;

        public UTCDateServiceTests()
        {
            _utcDateService = new UTCDateService();
        }

        [Fact]
        public void GetCurrentDate_ShouldReturnCurrentUTCDate()
        {
            // Arrange
            var expectedDate = DateTimeOffset.UtcNow.Date;

            // Act
            var result = _utcDateService.GetCurrentDate();

            // Assert
            Assert.Equal(expectedDate, result);
        }

        [Theory]
        [InlineData("20231001", "2023-10-01T00:00:00+00:00")]
        [InlineData("20231231", "2023-12-31T00:00:00+00:00")]
        public void ParseDate_ShouldReturnParsedUTCDate(string input, string expected)
        {
            // Act
            var result = _utcDateService.ParseDate(input);

            // Assert
            Assert.Equal(DateTimeOffset.Parse(expected), result);
        }
    }
}
