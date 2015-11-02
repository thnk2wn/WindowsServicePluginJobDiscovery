using System.ComponentModel.Composition;
using Cyrus.Plugin.Common;
using Hangfire;

namespace Cyrus.ClientMergeEmails
{
    [Export(typeof(IPlugin))]
    public class Plugin : IPlugin
    {
        public void ScheduleJobs()
        {
            RecurringJob.AddOrUpdate<MergeEmailsJob>(x=> x.Run(), Cron.Minutely);
            
        }
    }
}
