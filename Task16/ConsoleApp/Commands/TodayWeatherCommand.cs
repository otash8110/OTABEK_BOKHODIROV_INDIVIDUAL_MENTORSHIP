using BL;

namespace ConsoleApp.Commands
{
    internal class TodayWeatherCommand : ICommand
    {
        private readonly IWeatherService weatherService;
        public TodayWeatherCommand(IWeatherService weatherService) {
            this.weatherService = weatherService;
        }

        public async Task Execute()
        {
            Console.WriteLine("Enter city name to fetch a weather info:");
            var cityName = Console.ReadLine();
            var weatherResult = await weatherService.GetWeatherByCityNameAsync(cityName);

            Console.WriteLine(weatherResult);
            Console.WriteLine();
        }
    }
}
