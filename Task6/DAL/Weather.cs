namespace DAL
{
    public class Weather
    {
        public WeatherCoord Coord { get; set; }
        public WeatherMain Main { get; set; }
    }

    public class WeatherCoord
    {
        public float Lon { get; set; }

        public float Lat { get; set; }
    }

    public class WeatherMain
    {
        public float Temp { get; set; }
    }
}
