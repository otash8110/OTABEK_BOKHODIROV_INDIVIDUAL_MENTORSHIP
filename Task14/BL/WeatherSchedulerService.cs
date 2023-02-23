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

        public IEnumerable<WeatherHistory> GetFilteredWeatherHistory(string cityName, DateTime from, DateTime to)
        {
            if (from != DateTime.MinValue && to != DateTime.MinValue)
            {
                var result = weatherRepository.Filter(weather => weather.Time > from && weather.Time < to && weather.Name == cityName);
                return result;
            }
            else throw new ArgumentOutOfRangeException("Please, provide correct DateTime values");
        }

        public async Task<string> GetWeatherHistoryForPeriodReport(IEnumerable<string> cityNames, DateTime from, DateTime to)
        {
            if (from != DateTime.MinValue && to != DateTime.MinValue)
            {
                var finalString = $"The report was generated: {DateTime.Now}. Period: {from} - {to}\n";
                var allWeatherHistory = await weatherRepository.GetAll();
                var result = allWeatherHistory.Where(w => cityNames.Contains(w.Name))
                    .GroupBy(p => p.Name)
                        .Select(w => new WeatherHistory { Name = w.Key, Temperature = w.Average(i => i.Temperature) });

                foreach (var city in cityNames)
                {
                    if(result.Any(w => w.Name == city))
                    {
                        var oneAverageWeather = result.Where(w => w.Name == city).FirstOrDefault();
                        finalString += $"{city} average temperature: { oneAverageWeather.Temperature }\n";
                    }
                    else
                    {
                        finalString += $"{city}: no statistics.\n";
                    }
                }

                return finalString;
            }
            else throw new ArgumentOutOfRangeException("Please, provide correct DateTime values");
        }
    }
}
