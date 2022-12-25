namespace DAL
{
    public interface IWeatherManager
    {
        Task<WeatherResponse> FetchWeatherByCityName(string cityName);
    }
}
