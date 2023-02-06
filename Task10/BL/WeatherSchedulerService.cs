﻿using BL.HttpService;
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

        public async Task FetchAndSaveWeatherAsync(string cityName, CancellationToken cancellationToken)
        {
            var fetchWeather = await weatherHttpClient.FetchWeatherByCityNameAsync(cityName, cancellationToken);
            var weather = new WeatherHistory()
            {
                Temperature = fetchWeather.Main.Temp,
                Time = DateTime.Now,
            };
            await weatherRepository.Insert(weather);
        }
    }
}
