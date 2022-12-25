namespace DAL
{
    public class WeatherResponse
    {
        public WeatherMainElements Main { get; set; }
    }

    public class WeatherMainElements
    {
        public float Temp { get; set; }
    }
}
