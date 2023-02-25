using API.Services;
using BL.HttpService;
using BL.Options;
using BL.Validation;
using DAL;
using DAL.WeatherHistoryOptionsModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IWeatherHttpClient, WeatherHttpClient>();
            services.AddScoped<IWeatherRepository, WeatherRepository>();
            services.AddScoped<IValidation, ValidationService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddOptions();
            services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMqConfig"));
            services.Configure<CitiesOption>(configuration);

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:5001";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });

            return services;
        }
    }
}
