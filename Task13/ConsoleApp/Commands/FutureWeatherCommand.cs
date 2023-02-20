using BL;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp.Commands
{
    internal class FutureWeatherCommand : ICommand
    {
        private readonly IWeatherService weatherService;
        public FutureWeatherCommand(IWeatherService weatherService)
        {
            this.weatherService = weatherService;
        }
        public async Task Execute()
        {
            Console.WriteLine("Enter city name to fetch a weather info:");
            var cityName = Console.ReadLine();
            Console.WriteLine("Enter number of days");
            var days = Convert.ToInt32(Console.ReadLine());

            var weatherResult = await weatherService.GetFutureWeatherByCityNameAsync(cityName,
                days);

            Console.WriteLine(weatherResult);
            Console.WriteLine();
        }
    }
}
