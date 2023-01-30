using BL;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp.Commands
{
    internal class FutureWeatherCommand : ICommand
    {
        private readonly IWeatherService weatherService;
        private readonly IConfiguration configuration;
        public FutureWeatherCommand(IWeatherService weatherService,
            IConfiguration configuration)
        {
            this.weatherService = weatherService;
            this.configuration = configuration;
        }
        public async Task Execute()
        {
            Console.WriteLine("Enter city name to fetch a weather info:");
            var cityName = Console.ReadLine();
            Console.WriteLine("Enter number of days");
            var days = Convert.ToInt32(Console.ReadLine());

            var weatherResult = await weatherService.GetFutureWeatherByCityNameAsync(cityName,
                days,
                configuration);

            Console.WriteLine(weatherResult);
            Console.WriteLine();
        }
    }
}
