using RoomAvailability.DataInput.JsonFile.Models;

namespace RoomAvailability.DataInput.JsonFile.DTO
{
    internal class Hotel
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<RoomType> roomTypes { get; set; }
        public List<Room> rooms { get; set; }
    }
}
