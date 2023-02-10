namespace DAL
{
    public class WeatherForecast
    {
        public ForecastDay Forecast { get; set; }
    }

    public class ForecastDay
    {
        public List<Days> Forecastday { get; set; }
    }

    public class Days
    {
        public Day Day { get; set; }
    }
    public class Day
    {
        public float Avgtemp_c { get; set; }
    }
}
