using BL;
using BL.HttpService;
using BL.Validation;
using ConsoleApp.Commands;
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

            IValidation validation = new ValidationService();
            IWeatherRepository weatherRepository = new WeatherRepository();
            IWeatherHttpClient weatherManager = new WeatherHttpClient(configuration["weather-api-key"],
                configuration["weather-api-key-secondary"]);
            IWeatherService weatherService = new WeatherService(weatherManager,
                weatherRepository,
                validation);

            var todayWeatherCommand = new TodayWeatherCommand(weatherService);
            var futureWeatherCommand = new FutureWeatherCommand(weatherService, configuration);
            var maxTemperatureWeatherCommand = new MaxTemperatureWeatherCommand(weatherService, configuration);


            var invoker = new Invoker();


            while(flag)
            {
                try
                {
                    Console.WriteLine("1. Current weather\n" +
                        "2. Weather forecast\n" +
                        "3. Max weather\n" +
                        "0. Close application");
                    var response = Console.ReadLine();

                    ICommand command = response switch
                    {
                        "1" => todayWeatherCommand,
                        "2" => futureWeatherCommand,
                        "3" => maxTemperatureWeatherCommand,
                        "0" => null,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (command != null)
                    {
                        invoker.SetCommand(command);
                        await invoker.ExecuteCommand();
                    }
                    else 
                        flag = false;
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