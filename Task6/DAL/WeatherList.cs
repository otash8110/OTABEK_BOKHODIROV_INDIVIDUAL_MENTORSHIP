namespace DAL
{
    public class WeatherList
    {
        public List<WeatherListMain> List { get; set; }
    }

    public class WeatherListMain
    {
        public WeatherListTemp Temp { get; set; }
    }

    public class WeatherListTemp
    {
        public float Day { get; set; }
    }
}
