namespace RoomAvailability.Services
{
    /// <summary>
    /// Service responsible for hosting Room Availability Application logic
    /// </summary>
    public interface IRoomAvailabilityService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelsFilePath">Path to hotels info file</param>
        /// <param name="bookingsFilePath">Path to bookings info file</param>
        /// <returns></returns>
        Task Execute(string hotelsFilePath, string bookingsFilePath);
    }
}
