using BL;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp.Commands
{
    public class MaxTemperatureWeatherCommand : ICommand
    {
        private readonly IConfiguration configuration;
        private readonly IWeatherService weatherService;
        public MaxTemperatureWeatherCommand(IWeatherService weatherService,
            IConfiguration configuration)
        {
            this.weatherService = weatherService;
            this.configuration = configuration;
        }

        public async Task Execute()
        {
            Console.WriteLine("Enter city names separated with comma to fetch a weather info:");
            var cityNames = Console.ReadLine();
            var splitedCityNames = cityNames.Split(",", StringSplitOptions.RemoveEmptyEntries);
            //var weatherResult = await 

            //Console.WriteLine(weatherResult);
            Console.WriteLine();
        }
    }
}
