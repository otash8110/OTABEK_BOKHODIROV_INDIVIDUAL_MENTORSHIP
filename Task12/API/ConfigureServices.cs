using API.Services;
using BL.HttpService;
using BL.Validation;
using DAL;
using DAL.WeatherHistoryOptionsModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
            services.Configure<CitiesOption>(configuration);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddIdentityServerAuthentication(opt =>
                {
                    opt.ApiName = "API";
                    opt.Authority = "https://localhost:5001";
                    opt.RequireHttpsMetadata = false;
                });

            return services;
        }
    }
}
