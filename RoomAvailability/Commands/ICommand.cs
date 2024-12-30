namespace RoomAvailability.Commands
{
    /// <summary>
    /// Command interface
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Command ready flag.
        /// </summary>
        bool CommandReady { get; }

        /// <summary>
        /// Configure the command with the input if applicable
        /// </summary>
        /// <param name="input">Raw Command</param>
        /// <returns>Returns true if command has been set up with the input provided, other wise retruns false</returns>
        bool Configure(string input);

        /// <summary>
        /// Process the command
        /// </summary>
        /// <returns>Result of the command execution</returns>
        IAsyncEnumerable<string> Execute();
    }
}
