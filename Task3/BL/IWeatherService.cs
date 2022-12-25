namespace BL
{
    public interface IWeatherService
    {
        Task<string> GetWeatherByCityName(string cityName);
    }
}
