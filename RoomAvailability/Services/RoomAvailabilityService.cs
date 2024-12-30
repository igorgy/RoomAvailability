using Microsoft.Extensions.DependencyInjection;
using RoomAvailability.Commands;
using RoomAvailability.DataInput.JsonFile;

namespace RoomAvailability.Services
{
    /// <inheritdoc/>
    public class RoomAvailabilityService : IRoomAvailabilityService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDataInputProcessor _dataInputProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomAvailabilityService"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        /// <param name="dataInputProcessor">Data input processor</param>
        public RoomAvailabilityService(
            IServiceProvider serviceProvider,
            IDataInputProcessor dataInputProcessor)
        {
            _serviceProvider = serviceProvider;
            _dataInputProcessor = dataInputProcessor;
        }

        /// <inheritdoc/>
        public async Task Execute(string hotelsFilePath, string bookingsFilePath)
        {
            await ProcessInputFiles(hotelsFilePath, bookingsFilePath);

            while (true)
            {
                // read command from console
                Console.WriteLine("Waiting for command. Or type 'exit' to close the Application.");
                var input = Console.ReadLine();
                input = input?.Replace(" ", string.Empty);
                if (input == null || input == "exit")
                {
                    break;
                }

                // process the command
                try
                {
                    List<ICommand> setupCommands = _serviceProvider
                    .GetServices<ICommand>()
                    .Select(c => c)
                    .Where(c => c.Configure(input))
                    .ToList();

                    if (setupCommands.Count == 1)
                    {
                        await foreach (var item in setupCommands.Single().Execute())
                        {
                            Console.WriteLine(item);
                        }

                    }
                    // error out if no command found
                    else if (setupCommands.Count == 0)
                    {
                        Console.Error.WriteLine("No command found for the input provided.");
                    }
                    // error out if multiple commands found
                    else
                    {
                        Console.Error.WriteLine("Multiple commands found for the input provided.");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
            }
        }

        private async Task ProcessInputFiles(string hotelsFilePath, string bookingsFilePath)
        {
            try
            {
                await _dataInputProcessor.Process(hotelsFilePath, bookingsFilePath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
