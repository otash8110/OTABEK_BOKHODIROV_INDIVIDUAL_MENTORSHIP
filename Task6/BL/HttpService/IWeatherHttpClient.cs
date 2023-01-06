using DAL;

namespace BL.HttpService
{
    public interface IWeatherHttpClient
    {
        Task<Weather> FetchWeatherByCityNameAsync(string cityName);
        Task<WeatherForecast> FetchWeatherListByCoordsAsync(string cityName, int days);
    }
}
