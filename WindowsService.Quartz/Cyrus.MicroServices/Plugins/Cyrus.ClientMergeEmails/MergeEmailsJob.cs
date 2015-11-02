using System.Diagnostics;
using Quartz;

namespace Cyrus.ClientMergeEmails
{
    class MergeEmailsJob : IInterruptableJob
    {
        public const string ID = "MergeEmailsJob";

        public void Execute(IJobExecutionContext context)
        {
            Trace.WriteLine("Running merge emails job");
        }

        public void Interrupt()
        {
            Trace.WriteLine("TODO: interrupt");
        }
    }
}
