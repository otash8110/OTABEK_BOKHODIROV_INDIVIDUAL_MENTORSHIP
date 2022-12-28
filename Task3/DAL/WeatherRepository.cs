namespace DAL
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly List<Weather> listOfWeather;

        public WeatherRepository()
        {
            listOfWeather = new List<Weather>();
        }

        public void Insert(Weather weather)
        {
            listOfWeather.Add(weather);
        }
    }
}
