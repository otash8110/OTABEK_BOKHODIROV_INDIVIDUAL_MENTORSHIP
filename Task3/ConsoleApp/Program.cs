using BL;
using BL.HttpService;
using BL.Validation;
using DAL;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp
{
    internal class Program
    {
        private static IConfiguration configuration;
        private static bool flag = true;

        static async Task Main(string[] args)
        {
            SetupConfiguration();

            IValidation validation = new ValidationService();
            IWeatherRepository weatherRepository = new WeatherRepository();
            IWeatherHttpClient weatherManager = new WeatherHttpClient(configuration["weather-api-key"]);
            IWeatherService weatherService = new WeatherService(weatherManager, weatherRepository, validation);
            while(flag)
            {
                try
                {
                    Console.WriteLine("Enter city name to fetch a weather info:");
                    var cityName = Console.ReadLine();
                    var weatherResult = await weatherService.GetWeatherByCityNameAsync(cityName);

                    Console.WriteLine(weatherResult);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        static void SetupConfiguration() => configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
    }
}