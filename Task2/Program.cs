using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Task2
{
    class Program
    {
        private static IConfiguration configuration;

        static async Task Main(string[] args)
        {
            SetupConfiguration();
            var weatherManager = new WeatherManager(configuration["weather-api-key"]);

            var result = await weatherManager.GetWeatherByCityName("Tashkent");

            Console.WriteLine(result.Main.Temp);
        }

        static void SetupConfiguration()
        {
            configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        }
    }

    class WeatherManager
    {
        private string weatherEndpointURL = "https://api.openweathermap.org/data/2.5/weather";
        private string apiKey;

        public WeatherManager(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<WeatherResponse> GetWeatherByCityName(string cityName)
        {
            var fetchURL = $"{weatherEndpointURL}?q={cityName}&appid={apiKey}&units=metric";

            try
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage res = await client.GetAsync(fetchURL);
                using HttpContent content = res.Content;

                string data = await content.ReadAsStringAsync();

                if (data != null)
                {
                    var dataObject = JObject.Parse(data);

                    var objectDeserialized = dataObject.ToObject<WeatherResponse>();

                    return objectDeserialized;
                }

                return null;
            }
            catch (Exception )
            {
                throw;
            }
        }
    }
}
