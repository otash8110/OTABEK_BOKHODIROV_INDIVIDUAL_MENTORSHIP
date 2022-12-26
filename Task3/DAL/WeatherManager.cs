using Newtonsoft.Json.Linq;

namespace DAL
{
    public class WeatherManager: IWeatherManager
    {
        private string weatherEndpointURL = "https://api.openweathermap.org/data/2.5/weather";
        private string apiKey;

        public WeatherManager(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<WeatherResponse> FetchWeatherByCityNameAsync(string cityName)
        {
            var fetchURL = $"{weatherEndpointURL}?q={cityName}&appid={apiKey}&units=metric";

            try
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage res = await client.GetAsync(fetchURL);
                using HttpContent content = res.Content;

                string data = await content.ReadAsStringAsync();

                if (data != null && res.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var dataObject = JObject.Parse(data);

                    var objectDeserialized = dataObject.ToObject<WeatherResponse>();

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
