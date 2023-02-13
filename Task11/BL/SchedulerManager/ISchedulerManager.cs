namespace BL.SchedulerManager
{
    public interface ISchedulerManager
    {
        public Task ScheduleJobs(bool isReschedule);
    }
}
