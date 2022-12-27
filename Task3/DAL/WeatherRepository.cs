namespace DAL
{
    public class WeatherRepository : IWeatherRepository
    {
        private List<Weather> listOfWeather;
        public void Save(Weather weather)
        {
            listOfWeather.Add(weather);
        }
    }
}
