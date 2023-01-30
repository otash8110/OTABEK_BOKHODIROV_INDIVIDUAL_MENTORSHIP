
namespace DAL
{
    public class WeatherAsyncResult
    {
        public Weather Weather { get; set; }
        public long Time { get; set; }
        public string Error { get; set; }
        public string CityName { get; set; }
        public bool IsCancelled { get; set; } = false;
    }
}
