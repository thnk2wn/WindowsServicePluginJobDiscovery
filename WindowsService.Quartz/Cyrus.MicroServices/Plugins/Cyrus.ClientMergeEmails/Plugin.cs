using System.ComponentModel.Composition;
using Cyrus.Plugin.Common;

using Quartz;

namespace Cyrus.ClientMergeEmails
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        public void ScheduleJobs(IScheduler scheduler)
        {
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trgClientMergeEmails", "mergeEmails")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever())
                .Build();

            var jobDetail = JobBuilder.Create<MergeEmailsJob>()
                .WithIdentity("jobMergeEmails")
                .Build();

            scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
