using DAL;

namespace BL.HttpService
{
    public interface IWeatherHttpClient
    {
        Task<Weather> FetchWeatherByCityNameAsync(string cityName, CancellationToken cancellationToken);
        Task<WeatherForecast> FetchFutureWeatherAsync(string cityName, int days);
    }
}
