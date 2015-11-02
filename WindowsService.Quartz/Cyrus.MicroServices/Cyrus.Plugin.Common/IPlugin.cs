using Quartz;

namespace Cyrus.Plugin.Common
{
    public interface IPlugin
    {
        void ScheduleJobs(IScheduler scheduler);
    }
}
