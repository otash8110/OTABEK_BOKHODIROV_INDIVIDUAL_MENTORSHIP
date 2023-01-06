using BL;
using BL.HttpService;
using BL.Validation;
using DAL;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp
{
    internal class Program
    {
        public static IConfiguration configuration;
        private static bool flag = true;

        static async Task Main(string[] args)
        {
            SetupConfiguration();

            IValidation validation = new ValidationService(configuration);
            IWeatherRepository weatherRepository = new WeatherRepository();
            IWeatherHttpClient weatherManager = new WeatherHttpClient(configuration["weather-api-key"],
                configuration["weather-api-key-secondary"]);
            IWeatherService weatherService = new WeatherService(weatherManager, weatherRepository, validation);


            while(flag)
            {
                try
                {
                    Console.WriteLine("Enter city name to fetch a weather info:");
                    var cityName = Console.ReadLine();
                    var weatherResult = await weatherService.GetFutureWeatherByCityNameAsync(cityName, 0);

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
            .AddJsonFile("appsettings.json")
            .Build();
    }
}