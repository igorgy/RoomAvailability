using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoomAvailability.Data.Entity;

namespace RoomAvailability.Data
{
    public class RoomAvailabilityContext : DbContext
    {
        public RoomAvailabilityContext(DbContextOptions options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hotel>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Hotel>()
                .HasMany(x => x.Rooms)
                .WithOne()
                .HasForeignKey(x => x.HotelId);

            modelBuilder.Entity<Room>()
                .HasKey(x => new { x.HotelId, x.RoomType });

            modelBuilder.Entity<Booking>()
                .HasKey(x => new { x.Id });
            modelBuilder.Entity<Booking>()
                .HasOne(x => x.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => new {b.HotelId, b.RoomType});


            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
                // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
                // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
                // use the DateTimeOffsetToBinaryConverter
                // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
                // This only supports millisecond precision, but should be sufficient for most use cases.
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                                || p.PropertyType == typeof(DateTimeOffset?));
                    foreach (var property in properties)
                    {
                        modelBuilder
                            .Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(new DateTimeOffsetToBinaryConverter());
                    }
                }
            }
        }
    }
}
