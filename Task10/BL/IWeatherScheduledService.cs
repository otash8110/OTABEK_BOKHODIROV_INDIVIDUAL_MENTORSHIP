namespace BL
{
    public interface IWeatherScheduledService
    {
        Task FetchAndSaveWeatherAsync(string cityName);
    }
}
