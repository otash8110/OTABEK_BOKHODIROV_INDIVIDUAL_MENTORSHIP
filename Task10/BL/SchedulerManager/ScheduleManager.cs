using DAL.WeatherHistoryOptionsModels;
using Microsoft.Extensions.Options;

namespace BL.SchedulerManager
{
    public class ScheduleManager : ISchedulerManager
    {
        private IOptionsMonitor<CitiesOption> citiesOption;

        public ScheduleManager(IOptionsMonitor<CitiesOption> citiesOption)
        {
            this.citiesOption = citiesOption;
            this.citiesOption.OnChange(OnChangeCallback);
        }

        public void ScheduleJobs()
        {
            throw new NotImplementedException();
        }

        private void OnChangeCallback(CitiesOption citiesOption)
        {
            ScheduleJobs();
        }
    }
}
