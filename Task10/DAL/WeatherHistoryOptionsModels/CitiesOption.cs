namespace DAL.WeatherHistoryOptionsModels
{
    public class CitiesOption
    {
        public string CitiesName = "Cities";
        public IEnumerable<City> Cities { get; set; }
    }
}
