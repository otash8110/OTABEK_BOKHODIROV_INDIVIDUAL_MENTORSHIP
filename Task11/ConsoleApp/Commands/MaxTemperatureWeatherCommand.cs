using BL;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace ConsoleApp.Commands
{
    public class MaxTemperatureWeatherCommand : ICommand
    {
        private readonly IWeatherService weatherService;

        public MaxTemperatureWeatherCommand(IWeatherService weatherService)
        {
            this.weatherService = weatherService;
        }

        public async Task Execute()
        {
            Console.WriteLine("Enter city names separated with comma to fetch a weather info:");
            var cityNames = Console.ReadLine();
            var splitedCityNames = Regex.Split(cityNames, ", +");
            var weatherResult = await weatherService.GetMaxWeatherByCityNamesAsync(splitedCityNames);

            Console.WriteLine(weatherResult);
            Console.WriteLine();
        }
    }
}
