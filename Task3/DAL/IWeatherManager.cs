namespace DAL
{
    public interface IWeatherManager
    {
        Task<WeatherResponse> FetchWeatherByCityNameAsync(string cityName);
    }
}
