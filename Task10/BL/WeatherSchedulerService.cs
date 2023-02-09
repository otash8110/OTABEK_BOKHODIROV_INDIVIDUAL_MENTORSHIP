using BL.HttpService;
using DAL;
using DAL.WeatherHistoryEntity;

namespace BL
{
    public class WeatherSchedulerService : IWeatherScheduledService
    {
        private readonly IWeatherHistoryRepository weatherRepository;
        private readonly IWeatherHttpClient weatherHttpClient;
        public WeatherSchedulerService(IWeatherHistoryRepository weatherRepository,
            IWeatherHttpClient weatherHttpClient)
        {
            this.weatherRepository = weatherRepository;
            this.weatherHttpClient = weatherHttpClient;
        }

        public async Task FetchAndSaveManyWeathersAsync(IEnumerable<string> cityNames)
        {
            var tasks = new List<Task<Weather>>();
            foreach (var cityName in cityNames)
            {
                tasks.Add(weatherHttpClient.FetchWeatherByCityNameAsync(cityName, CancellationToken.None));
            }

            var results = await Task.WhenAll(tasks);

            var resultsAsHistory = results.Select(w => new WeatherHistory()
            {
                Name = w.Name,
                Temperature = w.Main.Temp,
                Time = DateTime.Now,
            });

            await weatherRepository.InsertMany(resultsAsHistory);
        }

        public async Task FetchAndSaveWeatherAsync(string cityName)
        {
            var fetchWeather = await weatherHttpClient.FetchWeatherByCityNameAsync(cityName, CancellationToken.None);

            var weather = new WeatherHistory()
            {
                Name = cityName,
                Temperature = fetchWeather.Main.Temp,
                Time = DateTime.Now,
            };
            await weatherRepository.Insert(weather);
        }
    }
}
