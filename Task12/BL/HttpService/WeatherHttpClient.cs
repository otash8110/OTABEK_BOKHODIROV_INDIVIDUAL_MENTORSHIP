using DAL;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net;

namespace BL.HttpService
{
    public class WeatherHttpClient: IWeatherHttpClient
    {
        private string weatherEndpointURL = "https://api.openweathermap.org/data/2.5/";
        private string apiKey;
        private readonly string apiKeyWeather;

        public WeatherHttpClient(IConfiguration configuration)
        {
            apiKey = configuration["weather-api-key"];
            apiKeyWeather = configuration["weather-api-key-secondary"];
        }

        public async Task<Weather> FetchWeatherByCityNameAsync(string cityName, CancellationToken cancellationToken)
        {
            var fetchURL = $"{weatherEndpointURL}/weather?q={cityName}&appid={apiKey}&units=metric";

            try
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage res = await client.GetAsync(fetchURL, cancellationToken);
                using HttpContent content = res.Content;

                string data = await content.ReadAsStringAsync();

                if (data != null && res.StatusCode != HttpStatusCode.NotFound)
                {
                    var dataObject = JObject.Parse(data);

                    var objectDeserialized = dataObject.ToObject<Weather>();
                    objectDeserialized.Name = cityName;
                    return objectDeserialized;
                }

                throw new Exception("City not found");
            }
            catch
            {
                throw;
            }
        }

        public async Task<WeatherForecast> FetchFutureWeatherAsync(string cityName, int days)
        {
            var fetchURL =
                $"http://api.weatherapi.com/v1/forecast.json?key={apiKeyWeather}&q={cityName}&days={days}&aqi=no&alerts=no";

            try
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage res = await client.GetAsync(fetchURL);
                using HttpContent content = res.Content;

                string data = await content.ReadAsStringAsync();

                if (data != null && res.StatusCode != HttpStatusCode.NotFound
                    && res.StatusCode != HttpStatusCode.BadRequest)
                {
                    var dataObject = JObject.Parse(data);

                    var objectDeserialized = dataObject.ToObject<WeatherForecast>();

                    return objectDeserialized;
                }

                return null;
            }
            catch
            {
                throw;
            }
        }
    }
}
