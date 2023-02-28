using API.Services;
using BL.HttpService;
using BL.Options;
using BL.Validation;
using DAL;
using DAL.WeatherHistoryOptionsModels;
using System.Security.Claims;

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
            services.Configure<EmailConfig>(configuration.GetSection("EmailConfig"));
            services.Configure<CitiesOption>(configuration);

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer",
                    jwtOptions =>
                    {
                        jwtOptions.Authority = "https://192.168.1.8:3094";
                        jwtOptions.RequireHttpsMetadata = false;
                        jwtOptions.RoleClaimType = ClaimTypes.Role;
                        jwtOptions.JwtBackChannelHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                  (message, certificate, chain, sslPolicyErrors) => true
                        };
                    });

            return services;
        }
    }
}
