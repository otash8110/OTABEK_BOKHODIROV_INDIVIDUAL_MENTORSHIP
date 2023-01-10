using Microsoft.Extensions.Configuration;

namespace BL
{
    public interface IWeatherService
    {
        Task<string> GetWeatherByCityNameAsync(string cityName);
        Task<string> GetFutureWeatherByCityNameAsync(string cityName, int days, IConfiguration configuration);
    }
}
