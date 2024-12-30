using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RoomAvailability.Core.Services;
using RoomAvailability.Data;
using RoomAvailability.Data.Entity;

namespace RoomAvailability.Tests.Core
{
    public class RoomAvailabilityTests
    {
        [Fact]
        public async Task GetHotelCapacity_ReturnsCorrectCapacity()
        {
            // Arrange
            using var context = GetInMemoryDatabase();
            context.Rooms.AddRange(
                new Room { HotelId = "Hotel1", RoomType = "SGL", Count = 10 },
                new Room { HotelId = "Hotel1", RoomType = "DBL", Count = 5 },
                new Room { HotelId = "Hotel2", RoomType = "SGL", Count = 8 }
            );
            context.SaveChanges();

            var service = new BookingService(context);

            // Act
            var capacity = await service.GetHotelCapacity("Hotel1", "SGL");

            // Assert
            Assert.Equal(10, capacity);
        }

        [Fact]
        public async Task GetHotelCapacity_ReturnsZero_WhenNoRoomsFound()
        {
            // Arrange
            using var context = GetInMemoryDatabase();
            var service = new BookingService(context);

            // Act
            var capacity = await service.GetHotelCapacity("NonExistentHotel", "SGL");

            // Assert
            Assert.Equal(0, capacity);
        }

        [Theory]
        [MemberData(nameof(GetBookingsCount_Data))]
        public async Task GetBookingsCount_ShouldReturnCorrectCount(
            Booking[] bookings,
            string hotelId,
            string roomType,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            int expectedResult)
        {
            // Arrange
            using var context = GetInMemoryDatabase();
            var service = new BookingService(context);

            context.Bookings.AddRange(bookings);
            await context.SaveChangesAsync();

            // Act
            var count = await service.GetBookingsCount(hotelId, roomType, startDate, endDate);

            // Assert
            Assert.Equal(expectedResult, count);
        }

        public static IEnumerable<object[]> GetBookingsCount_Data =>
            new List<object[]>
            {
                // 3 bookings withing the date range, 1 out of range
                new object[] { new Booking[]
                {
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 2)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 5)) },
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 8)) },
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 9)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 11)) },
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 11, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) }
                }, "hotel1", "SGL", new DateTimeOffset(new DateTime(2023, 10, 1)), new DateTimeOffset(new DateTime(2023, 10, 10)), 3 },
                // 1 booking withing the date range, 1 booking starting before the date range, 1 out of range
                new object[] { new Booking[]
                {
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 2)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 5)) },
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 09, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 8)) },
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 11, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) }
                }, "hotel1", "SGL", new DateTimeOffset(new DateTime(2023, 10, 1)), new DateTimeOffset(new DateTime(2023, 10, 10)), 2 },
                // 1 booking withing the date range, 1 booking ending after the date range, 1 out of range
                new object[] { new Booking[]
                {
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 2)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 5)) },
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) },
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 11, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) }
                }, "hotel1", "SGL", new DateTimeOffset(new DateTime(2023, 10, 1)), new DateTimeOffset(new DateTime(2023, 10, 10)), 2 },
                // 1 booking withing the date range, 1 booking overlaping the date range, 1 out of range
                new object[] { new Booking[]
                {
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 2)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 5)) },
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 09, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) },
                    new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 11, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) }
                }, "hotel1", "SGL", new DateTimeOffset(new DateTime(2023, 10, 1)), new DateTimeOffset(new DateTime(2023, 10, 10)), 2 },
            };

        [Theory]
        [MemberData(nameof(GetDailyBookingsCount_Data))]
        public async Task GetDailyBookingsCount_ShouldReturnCorrectCounts(
            Booking[] bookings,
            string hotelId,
            string roomType,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            int[] expectedResult)
        {
            // Arrange
            using var context = GetInMemoryDatabase();
            var service = new BookingService(context);

            context.Bookings.AddRange(bookings);

            await context.SaveChangesAsync();

            // Act
            var result = await service.GetDailyBookingsCount(hotelId, roomType, startDate, endDate);

            // Assert
            Assert.Equal(expectedResult.Length, result.Length);
            expectedResult.Should().BeEquivalentTo(result);
        }

        public static IEnumerable<object[]> GetDailyBookingsCount_Data =>
        new List<object[]>
        {
            // 3 bookings withing the date range, 1 out of range
            new object[] { new Booking[]
            {
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 1)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 3)) },
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 2)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 4)) },
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 3)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 5)) },
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 11, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) }
            }, "hotel1", "SGL", new DateTimeOffset(new DateTime(2023, 10, 1)), new DateTimeOffset(new DateTime(2023, 10, 5)), new int[]{ 1, 2, 2, 1} },
            // 1 booking withing the date range, 1 booking starting before the date range, 1 out of range
            new object[] { new Booking[]
            {
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 1)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 3)) },
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 09, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 4)) },
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 11, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) }
            }, "hotel1", "SGL", new DateTimeOffset(new DateTime(2023, 10, 1)), new DateTimeOffset(new DateTime(2023, 10, 5)), new int[]{ 2, 2, 1, 0} },
            // 1 booking withing the date range, 1 booking ending after the date range, 1 out of range
            new object[] { new Booking[]
            {
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 2)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 5)) },
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) },
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 11, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) }
            }, "hotel1", "SGL", new DateTimeOffset(new DateTime(2023, 10, 1)), new DateTimeOffset(new DateTime(2023, 10, 5)), new int[]{ 0, 1, 1, 2} },
            // 1 booking withing the date range, 1 booking overlaping the date range, 1 out of range
            new object[] { new Booking[]
            {
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 10, 2)), DepartureDate = new DateTimeOffset(new DateTime(2023, 10, 3)) },
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 09, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) },
                new Booking { HotelId = "hotel1", RoomType = "SGL", ArrivalDate = new DateTimeOffset(new DateTime(2023, 11, 4)), DepartureDate = new DateTimeOffset(new DateTime(2023, 11, 8)) }
            }, "hotel1", "SGL", new DateTimeOffset(new DateTime(2023, 10, 1)), new DateTimeOffset(new DateTime(2023, 10, 5)), new int[]{ 1, 2, 1, 1} },
        };

        private RoomAvailabilityContext GetInMemoryDatabase()
        {
            var options = new DbContextOptionsBuilder<RoomAvailabilityContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new RoomAvailabilityContext(options);
        }
    }
}