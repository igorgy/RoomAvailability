namespace RoomAvailability.DataInput.JsonFile
{
    /// <summary>
    /// Interface for processing input data.
    /// </summary>
    public interface IDataInputProcessor
    {
        /// <summary>
        /// Process input data.
        /// </summary>
        /// <param name="hotelsFilePath">Path to hotels info file</param>
        /// <param name="bookingsFilePath">Path to bookings info file</param>
        /// <returns></returns>
        Task Process(string hotelsFilePath, string bookingsFilePath);
    }
}
