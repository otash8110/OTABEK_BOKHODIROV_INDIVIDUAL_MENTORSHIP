using DAL;
using Newtonsoft.Json.Linq;

namespace BL.HttpService
{
    public class WeatherHttpClient: IWeatherHttpClient
    {
        private string weatherEndpointURL = "https://api.openweathermap.org/data/2.5/weather";
        private string apiKey;

        public WeatherHttpClient(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<Weather> FetchWeatherByCityNameAsync(string cityName)
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

                    var objectDeserialized = dataObject.ToObject<Weather>();

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
