namespace BL
{
    public interface IWeatherService
    {
        Task<string> GetWeatherByCityNameAsync(string cityName);
    }
}
