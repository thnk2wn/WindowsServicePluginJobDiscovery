using System.Diagnostics;

namespace Cyrus.ClientMergeEmails
{
    class MergeEmailsJob
    {
        public const string ID = "MergeEmailsJob";

        public void Run()
        {
            Trace.WriteLine("Running merge emails job");
        }
    }
}
