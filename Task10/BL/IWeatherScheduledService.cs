namespace BL
{
    public interface IWeatherScheduledService
    {
        Task FetchAndSaveWeatherAsync(string cityName);
        Task FetchAndSaveManyWeathersAsync(IEnumerable<string> cityNames);
    }
}
