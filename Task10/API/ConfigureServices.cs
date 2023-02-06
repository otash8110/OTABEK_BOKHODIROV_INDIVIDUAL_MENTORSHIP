using BL.HttpService;
using BL.Validation;
using DAL;
using DAL.WeatherHistoryOptionsModels;

namespace API
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IWeatherHttpClient, WeatherHttpClient>();
            services.AddScoped<IWeatherRepository, WeatherRepository>();
            services.AddScoped<IValidation, ValidationService>();

            services.AddOptions();
            services.Configure<CitiesOption>(configuration);

            return services;
        }
    }
}
