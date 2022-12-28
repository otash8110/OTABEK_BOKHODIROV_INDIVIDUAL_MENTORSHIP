namespace DAL
{
    public class WeatherRepository : IWeatherRepository
    {
        private List<Weather> listOfWeather;
        public void Insert(Weather weather)
        {
            listOfWeather.Add(weather);
        }
    }
}
