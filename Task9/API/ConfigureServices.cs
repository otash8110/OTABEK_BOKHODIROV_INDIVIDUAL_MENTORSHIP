using BL.HttpService;
using BL.Validation;
using DAL;

namespace API
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureApiServices(this IServiceCollection services)
        {
            services.AddScoped<IWeatherHttpClient, WeatherHttpClient>();
            services.AddScoped<IWeatherRepository, WeatherRepository>();
            services.AddScoped<IValidation, ValidationService>();

            return services;
        }
    }
}
